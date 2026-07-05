using UnityEngine;

[ExecuteAlways]
public class GridManager : MonoBehaviour
{
    [Header("Player Grid Settings")]
    [SerializeField] private Vector3 _playerBasePosition = new Vector3(-5f, -1f, 0f);
    
    [Header("Enemy Grid Settings")]
    [SerializeField] private Vector3 _enemyBasePosition = new Vector3(5f, -1f, 0f);

    [Header("Spacing Coordinates")]
    [Tooltip("Horizontal distance between columns")]
    [SerializeField] private float _colSpacing = 2.0f;
    [Tooltip("Vertical distance between rows")]
    [SerializeField] private float _rowSpacing = 1.5f;

    private int _maxRows = 3;
    private int _maxCols = 2;

    [Header("Editor Visualization")]
    [SerializeField] private Color _playerSlotColor = new Color(0f, 0.7f, 1f, 0.5f);
    [SerializeField] private Color _enemySlotColor = new Color(1f, 0.2f, 0.2f, 0.5f);
    [SerializeField] private float _slotGizmoSize = 2f;

    public Vector3 GetSlotWorldPosition(bool isPlayerTeam, int row, int col)
    {
        float finalColSpacing = _colSpacing <= 0 ? 2.0f : _colSpacing;
        float finalRowSpacing = _rowSpacing <= 0 ? 1.5f : _rowSpacing;

        if (isPlayerTeam)
        {
            float xOffset = -col * finalColSpacing;
            float yOffset = -row * finalRowSpacing;
            return _playerBasePosition + new Vector3(xOffset, yOffset, 0f);
        }
        else
        {
            float xOffset = col * finalColSpacing;
            float yOffset = -row * finalRowSpacing;
            return _enemyBasePosition + new Vector3(xOffset, yOffset, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _playerSlotColor;
        for (int r = 0; r < _maxRows; r++)
        {
            for (int c = 0; c < _maxCols; c++)
            {
                Vector3 position = GetSlotWorldPosition(true, r, c);
                Gizmos.DrawWireCube(position, new Vector3(_slotGizmoSize, _slotGizmoSize, 0.1f));
                
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(position + Vector3.up * (_slotGizmoSize * 0.6f), $"P [R:{r}, C:{c}]");
                #endif
            }
        }

        Gizmos.color = _enemySlotColor;
        for (int r = 0; r < _maxRows; r++)
        {
            for (int c = 0; c < _maxCols; c++)
            {
                Vector3 position = GetSlotWorldPosition(false, r, c);
                Gizmos.DrawWireCube(position, new Vector3(_slotGizmoSize, _slotGizmoSize, 0.1f));

                #if UNITY_EDITOR
                UnityEditor.Handles.Label(position + Vector3.up * (_slotGizmoSize * 0.6f), $"E [R:{r}, C:{c}]");
                #endif
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_playerBasePosition, 0.2f);
        Gizmos.DrawSphere(_enemyBasePosition, 0.2f);
    }
}