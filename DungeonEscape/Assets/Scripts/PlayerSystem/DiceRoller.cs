using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlayerSystem
{
    public class DiceRoller : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button rollButton;
        [SerializeField] private Image movementDiceImage;
        [SerializeField] private Image actionDiceImage;

        [Header("Dice Face Sprites (1-6)")]
        [Tooltip("Assign sprites for dice faces 1-6. If left empty, will try to load from Resources/DiceFaces/")]
        [SerializeField] private Sprite[] diceFaces; // Index 0 = face 1, etc.

        [Header("Roll Settings")]
        [SerializeField] private float rollDuration = 1.0f;
        [SerializeField] private float spinInterval = 0.08f;

        private bool isRolling = false;
        private int lastMovementResult = 0;
        private int lastActionResult = 0;

        private void Awake()
        {
            // Try to load dice faces from Resources if not assigned
            if (diceFaces == null || diceFaces.Length < 6)
            {
                LoadDiceFacesFromResources();
            }
        }

        private void Start()
        {
            if (rollButton != null)
            {
                rollButton.onClick.AddListener(RollDice);
            }

            // Show face 1 initially
            UpdateDiceDisplay(1, 1);
        }

        private void LoadDiceFacesFromResources()
        {
            diceFaces = new Sprite[6];
            string[] faceNames = { "diceOne", "diceTwo", "diceThree", "diceFour", "diceFive", "diceSix" };

            for (int i = 0; i < 6; i++)
            {
                diceFaces[i] = Resources.Load<Sprite>($"DiceFaces/{faceNames[i]}");
                if (diceFaces[i] == null)
                {
                    Debug.LogWarning($"[DiceRoller] Could not load sprite: DiceFaces/{faceNames[i]}");
                }
            }
        }

        public void RollDice()
        {
            if (isRolling) return;
            StartCoroutine(RollAnimation());
        }

        private IEnumerator RollAnimation()
        {
            isRolling = true;
            if (rollButton != null) rollButton.interactable = false;

            float elapsed = 0f;

            // Animate dice spinning
            while (elapsed < rollDuration)
            {
                int randomMove = Random.Range(1, 7);
                int randomAction = Random.Range(1, 7);
                UpdateDiceDisplay(randomMove, randomAction);

                elapsed += spinInterval;
                yield return new WaitForSeconds(spinInterval);
            }

            // Final result
            lastMovementResult = Random.Range(1, 7);
            lastActionResult = Random.Range(1, 7);
            UpdateDiceDisplay(lastMovementResult, lastActionResult);

            Debug.Log($"[DiceRoller] Rolled: Movement={lastMovementResult}, Action={lastActionResult}");
            PlayerEventSystem.TriggerDiceRolled(lastMovementResult, lastActionResult);

            isRolling = false;
            if (rollButton != null) rollButton.interactable = true;
        }

        private void UpdateDiceDisplay(int movementValue, int actionValue)
        {
            if (diceFaces != null && diceFaces.Length >= 6)
            {
                if (movementDiceImage != null)
                    movementDiceImage.sprite = diceFaces[Mathf.Clamp(movementValue - 1, 0, 5)];
                if (actionDiceImage != null)
                    actionDiceImage.sprite = diceFaces[Mathf.Clamp(actionValue - 1, 0, 5)];
            }
        }

        public int GetLastMovementResult() => lastMovementResult;
        public int GetLastActionResult() => lastActionResult;
    }
}