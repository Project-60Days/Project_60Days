using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Hexamap
{
    // This script is used by the demo scene and is in charge of handling the camera controls and mouse selection
    public class DemoController : MonoBehaviour
    {
        private Camera _camera;
        private List<TileController> _selectedTiles = new List<TileController>();

        public HexamapController Hexamap;
        public Text TextStats;
        public GameObject playerPrefab;
        public GameObject player;
        public Tile playerLocationTile;

        public int MaxYPosition = 70;
        public int MinYPosition = 5;
        public int MinXRotation = 30;
        public int MaxXRotation = 90;
        public int ScrollSpeed = 50;
        public int MoveSpeed = 50;

        private bool isSelected;
        private bool playerCanMove;

        void Start()
        {
            _camera = Camera.main;
            generateMap();
        }

        void Update()
        {
            // -- Regenerate
            if (Input.GetKeyDown(KeyCode.R))
            {
                Destroy(player);
                generateMap();
            }

            CameraMoveInputKey();

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

            /* 
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
            */

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

                if (tile != null && !_selectedTiles.Contains(tile))
                {
                    // If the landform is a filler, just select the tile
                    if (tile.Model.Landform.GetType().IsSubclassOf(typeof(LandformFiller)))
                    {
                        unselectAllTile();
                        selectTile(tile);
                    }
                    else // If not, select the whole metalandform
                    {
                        selectMetaLandform(tile);
                    }
                }
            }
            else
            {
                unselectAllTile();
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray2 = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskPlayer) && !playerCanMove)
                {
                    isSelected = true;

                    // 플레이어 주변 보더 활성화
                    Debug.Log("플레이어! " + playerLocationTile.Coords);

                    foreach (KeyValuePair<CompassPoint, Tile> item in playerLocationTile.Neighbours)
                    {
                        var border = getTileBorder(item.Value);
                        border?.SetActive(true);
                    }
                    playerCanMove = true;
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskTile) && !isSelected)
                {
                    // 클릭한 타일의 좌표 출력
                    if (_selectedTiles[0].Coords != playerLocationTile.Coords.ToVector())
                        Debug.Log(_selectedTiles[0].Coords);
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskTile) && playerCanMove)
                {
                    Transform objectHit = hit.transform;
                    TileController tile = objectHit.parent.GetComponent<TileController>();
                    if (getTileControllerBorder(tile).activeInHierarchy)
                    {
                        //보더 비활성화
                        foreach (KeyValuePair<CompassPoint, Tile> item in playerLocationTile.Neighbours)
                        {
                            var border = getTileBorder(item.Value);
                            border?.SetActive(false);
                        }
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

            var tileToUnselect = _selectedTiles.Except(metaLandformTiles).ToList();
            var tileToSelect = metaLandformTiles.Except(_selectedTiles).ToList();

            tileToSelect.ForEach(t => selectTile(t));
            tileToUnselect.ForEach(t => unselectTile(t));
        }

        private void unselectAllTile()
        {
            foreach (var t in _selectedTiles)
            {
                if (t == null)
                    continue;

                GameObject border = getTileControllerBorder(t);
                if (border != null)
                    border.SetActive(false);
            }
            _selectedTiles.Clear();
        }

        private void unselectTile(TileController tile)
        {
            GameObject border = getTileControllerBorder(tile);
            if (border != null)
                border.SetActive(false);
            _selectedTiles.Remove(tile);


        }

        private void selectTile(TileController tile)
        {
            GameObject border = getTileControllerBorder(tile);

            if (border == null)
                return;

            border.SetActive(true);
            _selectedTiles.Add(tile);
        }

        private GameObject getTileControllerBorder(TileController tile)
        {
            GameObject tileGO = (GameObject)tile.Model.GameEntity;

            if (tileGO != null && tile.Model.Landform.GetType().Name != "LandformWorldLimit")
                return tileGO.transform.Find("Border").gameObject;

            return null;
        }

        private IEnumerator PlayerMove(Vector3 targetPos, float time = 0.5f)
        {
            
            yield return player.transform.DOMove(targetPos, time);
            isSelected = false;
            playerCanMove = false;

        }

        private GameObject getTileBorder(Tile tile)
        {
            GameObject tileGO = (GameObject)tile.GameEntity;

            if (tileGO != null && tile.Landform.GetType().Name != "LandformWorldLimit")
                return tileGO.transform.Find("Border").gameObject;

            return null;
        }

    }
}