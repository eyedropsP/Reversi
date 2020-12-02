using OneMonoBehaviourReversi.Enums;
using UnityEngine;

namespace OneMonoBehaviourReversi
{
    public class Reversi : MonoBehaviour
    {
        [SerializeField] private GameObject gridPrefab = default;
        [SerializeField] private GameObject gridBackgroundPrefab = default;
        [SerializeField] private GameObject whiteStone = default;
        [SerializeField] private GameObject blackStone = default;

        private SpriteRenderer gridSprite;
        private GameState gameState;
        
        private const int boardSize = 8;
        private const float endPoint = 3.85f;
        private const float interval = 1.1f;

        private int[,] grid;
        private GameObject[,] gridGameObject;

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
            gameState = GameState.TurnBlack;    
            gridGameObject = new GameObject[boardSize,boardSize];
            grid = new int[boardSize,boardSize];
            grid[3, 4] = grid[4,3] = -1;
            grid[3, 3] = grid[4, 4] = 1;
        }
    
        private void CreateBoard()
        {
            Instantiate(gridBackgroundPrefab);
        
            for (var h = 0; h < boardSize; h++)
            {
                for (var w = 0; w < boardSize; w++)
                {
                    var y = (h * interval) - endPoint;
                    var x = (w * interval) - endPoint;
                    gridGameObject[h, w] = Instantiate(gridPrefab, new Vector2(x, y), Quaternion.identity);
                
                    if (grid[h, w] == -1)
                    {
                        Instantiate(whiteStone, new Vector2(x, y), Quaternion.identity);
                    }

                    if (grid[h, w] == 1)
                    {
                        Instantiate(blackStone, new Vector2(x, y), Quaternion.identity);
                    }
                }
            }
        }

        private void RefreshBoard()
        {
            for (var h = 0; h < boardSize; h++)
            {
                for (var w = 0; w < boardSize; w++)
                {
                
                }
            }
        }

        private bool CanTransitionNextTurn()
        {
            for (var index0 = 0; index0 < grid.GetLength(0); index0++)
            for (var index1 = 0; index1 < grid.GetLength(1); index1++)
            {
                var gridState = grid[index0, index1];
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
                if (!hitObject.CompareTag("GridData"))
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
                
                if (gameState == GameState.TurnBlack)
                {
                    Instantiate(blackStone, hitObject.transform);
                    gameState = GameState.TurnWhite;
                    return;
                }
            
                if(gameState == GameState.TurnWhite)
                {
                    Instantiate(whiteStone, hitObject.transform);
                    gameState = GameState.TurnBlack;
                }
            }
        }
    }
}