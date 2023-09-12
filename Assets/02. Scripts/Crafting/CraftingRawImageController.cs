using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class CraftingRawImageController : MonoBehaviour, IDragHandler
{
    [SerializeField] Camera renderCamera;
    [SerializeField] Transform targetTr;
    [SerializeField] GameObject targetObject;
    [SerializeField] Transform generationPoint;
    [SerializeField] ItemSO ItemSO;

    [Header("Speed")]
    [SerializeField] float rotateSpeed = 10f;
    [SerializeField] float translateSpeed = 5.0f;
    [SerializeField] float zoomSpeed = 5.0f;

    [Header("Movement Limit")]
    [SerializeField] float maxCameraSize = 10.0f;
    [SerializeField] float minCameraSize = 2.0f;
    [SerializeField] float maxTargetPosition = 5.0f;
    [SerializeField] float rotateLimitAngle = 70.0f;

    private float camVelocity = 0.0F;
    private float camTargetSize = 0.0f;
    bool isControlKeyPushed;
    public bool canRotate;
    bool isTargetOn;

    void Start()
    {
        ItemSlot.InventoryItemClick += ChangerTarget;
        CraftSlot.CraftItemClick += DestroyObject;
    }

    void OnDestroy()
    {
        ItemSlot.InventoryItemClick -= ChangerTarget;
        CraftSlot.CraftItemClick -= DestroyObject;
    }

    IEnumerator VariablesConnect()
    {
        yield return new WaitForEndOfFrame();
        renderCamera = GameObject.FindGameObjectWithTag("RenderTextureCamera").GetComponent<Camera>();
        targetTr = targetObject.transform;
        camTargetSize = renderCamera.orthographicSize;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) isControlKeyPushed = true;
        if (Input.GetKeyUp(KeyCode.LeftControl)) isControlKeyPushed = false;

        if (isControlKeyPushed)
        {
            // -- Mouse scrolling
            float scrollWheel = (Input.GetAxis("Mouse ScrollWheel") * 5000 * Time.deltaTime);
            Zoom(scrollWheel);
        }
    }

    private void Zoom(float inputValue)
    {
        float currentSize = renderCamera.orthographicSize;
        camTargetSize = Mathf.Clamp(camTargetSize - (inputValue * zoomSpeed * Time.deltaTime), minCameraSize, maxCameraSize);

        renderCamera.orthographicSize = Mathf.SmoothDamp(currentSize, camTargetSize, ref camVelocity, 0.15f);
    }

    public float Zoom(float _targetSize, float _lastZoomSpeed)
    {
        camTargetSize = Mathf.SmoothDamp(renderCamera.orthographicSize, _targetSize, ref _lastZoomSpeed, 0.3f);
        renderCamera.orthographicSize = camTargetSize;

        return _lastZoomSpeed;
    }

    public Vector3 Translate(Vector3 _targetPos, Vector3 _lastMoveSpeed)
    {
        targetTr.transform.position = Vector3.SmoothDamp(targetTr.transform.position, _targetPos, ref _lastMoveSpeed, 0.3f);
        return _lastMoveSpeed;
    }

    public void Translate(Vector2 _inputValue)
    {
        Vector3 clampedPosition;

        Vector3 inputValue = new Vector3(_inputValue.x, 0, _inputValue.y);// = GetInputTranslationDirection();
        targetTr.Translate(inputValue * Time.deltaTime * translateSpeed);

        clampedPosition = targetTr.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -maxTargetPosition, maxTargetPosition);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, -maxTargetPosition, maxTargetPosition);
        targetTr.position = clampedPosition;

    }

    public void Rotate(float mouseX, float mouseY)
    {
        targetTr.Rotate(Vector3.down, mouseX);
        targetTr.Rotate(Vector3.right, mouseY);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isControlKeyPushed)
            return;

        float x = eventData.delta.x * Time.deltaTime * rotateSpeed;
        float y = eventData.delta.y * Time.deltaTime * rotateSpeed;

        targetTr.Rotate(0, -x, y, Space.World);
    }

    public void ChangerTarget(GameObject itemPrefab)
    {
        Destroy(targetObject);
        generationPoint = GameObject.FindGameObjectWithTag("GeneratePoint").GetComponent<Transform>();
        var newTargetObject = Instantiate(itemPrefab, generationPoint);

        targetObject = newTargetObject;
        StartCoroutine(VariablesConnect());
    }

    public void DestroyObject()
    {
        Destroy(targetObject);
    }
}