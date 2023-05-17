using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class Nodes
{
    public Nodes(bool _isWall, int _x, int _y)
    {
        isWall = _isWall;
        x = _x;
        y = _y;
    }

    public bool isWall;
    public Nodes ParentNode;

    // G : 시작으로부터 이동했던 거리, H : |가로|+|세로| 장애물 무시하여 목표까지의 거리, F : G + H
    public int x, y, G, H;
    public int F { get { return G + H; } }
}

namespace Hexamap
{
    // This script is used by the demo scene and is in charge of handling the camera controls and mouse selection
    public class DemoController : MonoBehaviour
    {
        private Camera _camera;
        private List<TileController> selectedTiles = new List<TileController>();
        List<Coords> selectPath;
        List<Coords> movePath;

        public HexamapController Hexamap;
        public Text TextStats;
        public Text TextHealth;
        public GameObject playerPrefab;
        public GameObject player;
        public Tile playerLocationTile;

        public int MaxYPosition = 70;
        public int MinYPosition = 5;
        public int MinXRotation = 30;
        public int MaxXRotation = 90;
        public int ScrollSpeed = 50;
        public int MoveSpeed = 50;
        public int Hp = 3;

        private bool isSelected;
        private bool isPlayerMove;
        private bool isUIOn;
        private bool playerCanMove;

        public GameObject currentUI;

        void Start()
        {
            _camera = Camera.main;
            generateMap();
        }

        void Update()
        {
            TextHealth.text = "체력: " + Hp;
            // -- Regenerate
            if (Input.GetKeyDown(KeyCode.R))
            {
                Destroy(player);
                generateMap();
            }

            CameraMoveInputKey();

            if (!isPlayerMove)
                TileSelectWithRaycast();
        }

        private void generateMap()
        {
            Hexamap.Destroy();

            var timeBefore = DateTime.Now;

            Hexamap.Generate();

            double timeSpent = (DateTime.Now - timeBefore).TotalSeconds;

            Hexamap.Draw();

            // Add some noise to Y position of tiles
            FastNoise _fastNoise = new FastNoise();
            _fastNoise.SetFrequency(0.1f);
            _fastNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
            _fastNoise.SetSeed(Hexamap.Map.Seed);
            foreach (Tile tile in Hexamap.Map.Tiles)
            {
                var noiseY = _fastNoise.GetValue(tile.Coords.X, tile.Coords.Y);
                (tile.GameEntity as GameObject).transform.position += new Vector3(0, noiseY * 2, 0);
            }

            // Output stats
            TextStats.text = $"Map generated in {timeSpent.ToString("0.000")} seconds.";
            Debug.Log($"Seed : { Hexamap.Map.Seed }");

            SpawnPlayer();
            unselectAllTile();
        }

        private void SpawnPlayer()
        {
            int randomInt;
            var tileList = Hexamap.Map.Tiles;

            while (true)
            {
                randomInt = UnityEngine.Random.Range(0, tileList.Count);

                if (tileList[randomInt].Landform.GetType().Name == "LandformPlain")
                {
                    break;
                }
            }

            playerLocationTile = tileList[randomInt];
            Vector3 spawnPos = ((GameObject)playerLocationTile.GameEntity).transform.position;
            spawnPos.y += 0.7f;

            player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        }

        private void CameraMoveInputKey()
        {
            // -- Keyboard movement
            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");

            Vector3 movement = _camera.transform.position;

            if (vertical > 0) // Top
            {
                movement += Vector3.forward * MoveSpeed * Time.deltaTime;
            }
            else if (vertical < 0) // Bottom
            {
                movement += Vector3.back * MoveSpeed * Time.deltaTime;
            }

            if (horizontal > 0) // Right
            {
                movement += Vector3.right * MoveSpeed * Time.deltaTime;
            }
            else if (horizontal < 0) // Left
            {
                movement += Vector3.left * MoveSpeed * Time.deltaTime;
            }


            // -- Mouse scrolling
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

            if (scrollWheel > 0 && movement.y > MinYPosition)
            {
                movement += Vector3.down * ScrollSpeed * Time.deltaTime;
            }
            else if (scrollWheel < 0 && movement.y < MaxYPosition)
            {
                movement += Vector3.up * ScrollSpeed * Time.deltaTime;
            }


            float relativeY = movement.y / (MaxYPosition - MinYPosition);
            float newXRot = (MaxXRotation - MinXRotation) * relativeY + MinXRotation;

            if (newXRot < MaxXRotation && newXRot > MinXRotation)
                _camera.transform.eulerAngles = new Vector3(newXRot, 0, 0);

            _camera.transform.position = movement;

        }

        private void TileSelectWithRaycast()
        {
            // Mouse raytracing to select tiles
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            int layerMaskTile = (-1) - (1 << LayerMask.NameToLayer("Player"));
            int layerMaskPlayer = (-1) - (1 << LayerMask.NameToLayer("Tile"));

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskTile) && !isSelected)
            {
                Transform objectHit = hit.transform;
                TileController tile = objectHit.parent.GetComponent<TileController>();



                if (tile != null && !selectedTiles.Contains(tile))
                {
                    // If the landform is a filler, just select the tile
                    if (tile.Model.Landform.GetType().IsSubclassOf(typeof(LandformFiller)))
                    {        
                        unselectAllTile();
                        selectTile(tile, "BorderNo");

                        if (isUIOn)
                        {
                            currentUI.SetActive(false);
                            isUIOn = false;
                        }
                    }
                    else // If not, select the whole metalandform
                    {
                        selectMetaLandform(tile);
                    }
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskTile) && playerCanMove)
            {
                Transform objectHit = hit.transform;
                TileController tile = objectHit.parent.GetComponent<TileController>();
                var hpCount = 0;

                unselectAllTile();
                unselectAllPathTile();

                if (tile.Model != playerLocationTile)
                {
                    selectPath = AStar.FindPath(playerLocationTile.Coords, tile.Model.Coords);

                    foreach (var item in selectPath)
                    {
                        if (hpCount == Hp)
                            break;

                        Tile targetTile = Hexamap.Map.GetTileFromCoords(item);
                        var tileBorder = getTileBorder(targetTile, "Border");
                        tileBorder?.SetActive(true);
                        hpCount++;
                    }
                }

                if (hpCount == Hp)
                    selectTile(tile, "BorderNo");
                else
                    selectTile(tile, "BorderYes");
            }
            else
            {
                unselectAllTile();
                unselectAllPathTile();
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray2 = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskPlayer) && !playerCanMove)
                {
                    isSelected = true;
                    playerCanMove = true;

                    // 플레이어 주변 보더 활성화
                    Debug.Log("플레이어의 좌표 : " + playerLocationTile.Coords);
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskTile) && !isSelected)
                {
                    // 클릭한 타일의 좌표 출력
                    /*                    if (selectedTiles[0].Coords != playerLocationTile.Coords.ToVector())
                                            Debug.Log(selectedTiles[0].Coords);*/


                    Transform objectHit = hit.transform;
                    TileController tile = objectHit.parent.GetComponent<TileController>();

                    currentUI = GetUi(tile);
                    currentUI.transform.position = Camera.main.WorldToScreenPoint(objectHit.position);
                    currentUI.transform.position += Vector3.right*300;
                    currentUI.transform.position += Vector3.up*200;
                    currentUI.SetActive(true);

                    isUIOn = true;
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskTile) && playerCanMove)
                {
                    Transform objectHit = hit.transform;
                    TileController tile = objectHit.parent.GetComponent<TileController>();
                    if (getTileControllerBorder(tile, "BorderYes").activeInHierarchy)
                    {

                        movePath = AStar.FindPath(playerLocationTile.Coords, tile.Model.Coords);
                        // 플레이어 선택한 칸으로 이동
                        StartCoroutine(PlayerMove(tile.transform.position));
                        playerLocationTile = tile.Model;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (playerCanMove)
                {
                    unselectAllTile();
                    unselectAllPathTile();
                    isSelected = false;
                    playerCanMove = false;
                }
            }
        }

        private void selectMetaLandform(TileController tile)
        {
            // Select metalandform of a tile
            var metaLandformTiles = tile
                .Model
                .Landform
                .MetaLandform
                .Tiles
                .Select(t => t.GameEntity)
                .Cast<GameObject>()
                .Select(g => g.GetComponent<TileController>())
                .ToList();

            var tileToUnselect = selectedTiles.Except(metaLandformTiles).ToList();
            var tileToSelect = metaLandformTiles.Except(selectedTiles).ToList();

            tileToSelect.ForEach(t => selectTile(t, "BorderNo"));
            tileToUnselect.ForEach(t => unselectTile(t));
        }

        private void unselectAllTile()
        {
            foreach (var t in selectedTiles)
            {
                if (t == null)
                    continue;
                BordersOff(t);
            }
            selectedTiles.Clear();
        }

        private void unselectAllPathTile()
        {
            if (selectPath == null)
                return;

            foreach (var t in selectPath)
            {
                BordersOff(Hexamap.Map.GetTileFromCoords(t));
            }
            selectPath.Clear();
        }

        private void unselectTile(TileController tile)
        {
            BordersOff(tile);
            selectedTiles.Remove(tile);
        }

        private void selectTile(TileController tile, string name)
        {
            GameObject border = getTileBorder(tile.Model, name);

            border.SetActive(true);
            selectedTiles.Add(tile);
        }

        private GameObject getTileControllerBorder(TileController tile, string name)
        {
            GameObject tileGO = (GameObject)tile.Model.GameEntity;

            if (tileGO != null && tile.Model.Landform.GetType().Name != "LandformWorldLimit")
                return tileGO.transform.Find(name).gameObject;

            return null;
        }

        private GameObject getTileBorder(Tile tile, string name)
        {
            GameObject tileGO = (GameObject)tile.GameEntity;

            if (tileGO != null && tile.Landform.GetType().Name != "LandformWorldLimit")
                return tileGO.transform.Find(name).gameObject;

            return null;
        }

        private GameObject GetUi(TileController tile)
        {
            GameObject tileGO = (GameObject)tile.Model.GameEntity;

            if (tileGO != null && tile.Model.Landform.GetType().Name != "LandformWorldLimit")
                return tileGO.transform.Find("Canvas").Find("TileInfo").gameObject;

            return null;
        }

        private void BordersOff(TileController tile)
        {
            GameObject border1 = getTileControllerBorder(tile, "Border");
            GameObject border2 = getTileControllerBorder(tile, "BorderYes");
            GameObject border3 = getTileControllerBorder(tile, "BorderNo");

            border1?.SetActive(false);
            border2?.SetActive(false);
            border3?.SetActive(false);
        }

        private void BordersOff(Tile tile)
        {
            GameObject border1 = getTileBorder(tile, "Border");
            GameObject border2 = getTileBorder(tile, "BorderYes"); ;
            GameObject border3 = getTileBorder(tile, "BorderNo"); ;

            border1?.SetActive(false);
            border2?.SetActive(false);
            border3?.SetActive(false);
        }

        private IEnumerator PlayerMove(Vector3 lastTargetPos, float time = 0.5f)
        {
            isPlayerMove = true;
            Tile targetTile;
            Vector3 targetPos;

            foreach (var item in movePath)
            {
                targetTile = Hexamap.Map.GetTileFromCoords(item);
                if (targetTile == null)
                    break;

                targetPos = ((GameObject)targetTile.GameEntity).transform.position;
                targetPos.y += 1;
                player.transform.DOMove(targetPos, time);
                yield return new WaitForSeconds(time);
            }
            lastTargetPos.y += 1;
            yield return player.transform.DOMove(lastTargetPos, time);

            isSelected = false;
            playerCanMove = false;
            isPlayerMove = false;
            unselectAllPathTile();
            movePath.Clear();
        }
    }
}