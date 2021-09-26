using OneMonoBehaviourReversi.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace OneMonoBehaviourReversi
{
    public class Reversi : MonoBehaviour
    {
        [FormerlySerializedAs("gridPrefab")] [SerializeField] private GameObject _gridPrefab;
        [FormerlySerializedAs("gridBackgroundPrefab")] [SerializeField] private GameObject _gridBackgroundPrefab;
        [FormerlySerializedAs("whiteStone")] [SerializeField] private GameObject _whiteStone;
        [FormerlySerializedAs("blackStone")] [SerializeField] private GameObject _blackStone;

        private SpriteRenderer _gridSprite;
        private GameState _gameState;
        
        private const int BoardSize = 8;
        private const float EndPoint = 3.85f;
        private const float Interval = 1.1f;

        private int[,] _grid;
        private GameObject[,] _gridGameObject;

        public Reversi(SpriteRenderer gridSprite)
        {
            _gridSprite = gridSprite;
        }

        // Start is called before the first frame update
        public void Start()
        {
            InitializeGame();
            CreateBoard();
        }

        // Update is called once per frame
        private void Update()
        {
            PutStone();
        }

        private void InitializeGame()
        {
            _gameState = GameState.TurnBlack;    
            _gridGameObject = new GameObject[BoardSize,BoardSize];
            _grid = new int[BoardSize,BoardSize];
            _grid[3, 4] = _grid[4,3] = -1;
            _grid[3, 3] = _grid[4, 4] = 1;
        }
    
        private void CreateBoard()
        {
            Instantiate(_gridBackgroundPrefab);
        
            for (var h = 0; h < BoardSize; h++)
            {
                for (var w = 0; w < BoardSize; w++)
                {
                    var y = (h * Interval) - EndPoint;
                    var x = (w * Interval) - EndPoint;
                    _gridGameObject[h, w] = Instantiate(_gridPrefab, new Vector2(x, y), Quaternion.identity);
                
                    if (_grid[h, w] == -1)
                    {
                        Instantiate(_whiteStone, new Vector2(x, y), Quaternion.identity);
                    }

                    if (_grid[h, w] == 1)
                    {
                        Instantiate(_blackStone, new Vector2(x, y), Quaternion.identity);
                    }
                }
            }
        }

        private void RefreshBoard()
        {
            for (var h = 0; h < BoardSize; h++)
            {
                for (var w = 0; w < BoardSize; w++)
                {
                
                }
            }
        }

        private bool CanTransitionNextTurn()
        {
            for (var index0 = 0; index0 < _grid.GetLength(0); index0++)
            for (var index1 = 0; index1 < _grid.GetLength(1); index1++)
            {
                var gridState = _grid[index0, index1];
                if (gridState == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void PutStone()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return; 
            }
            
            var raycastHit2D = new RaycastHit2D();
            if (!(Camera.main is null))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);
            }

            if (raycastHit2D)
            {
                var hitObject = raycastHit2D.transform.gameObject;
                if (!hitObject.CompareTag($"GridData"))
                {
                    return;
                }

                // for (var h = 0; h < boardSize; h++)
                // {
                //     for (var w = 0; w < boardSize; w++)
                //     {
                //         if (grid[h, w] != 0)
                //         {
                //             return;
                //         }
                //     }
                // }
                
                if (_gameState == GameState.TurnBlack)
                {
                    Instantiate(_blackStone, hitObject.transform);
                    _gameState = GameState.TurnWhite;
                    return;
                }
            
                if(_gameState == GameState.TurnWhite)
                {
                    Instantiate(_whiteStone, hitObject.transform);
                    _gameState = GameState.TurnBlack;
                }
            }
        }
    }
}