using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRadius = 16f; // 16픽셀
    [SerializeField] private LayerMask npcLayer;
    
    private CircleCollider2D interactionCollider;
    private Player playerMovement;
    private NPC currentNPC;
    private bool canInteract = false;

    void Start()
    {
        interactionCollider = GetComponent<CircleCollider2D>();
        interactionCollider.radius = interactionRadius;
        interactionCollider.isTrigger = true;
    
        playerMovement = GetComponent<Player>();
    }

    void Update()
    {
        // Z키 입력 감지
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (canInteract && currentNPC != null)
            {
                // 대화 진행 중이 아니면 대화 시작
                if (!DialogueManager.Instance.IsDialogueActive())
                {
                    InteractWithNPC();
                }
                else
                {
                    // 대화 진행 중이면 다음 대사 또는 즉시 표시
                    DialogueManager.Instance.AdvanceDialogue();
                }
            }
        }

        // 대화 상태에 따라 플레이어 이동 제어
        if (playerMovement != null)
        {
            bool isDialogueActive = DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive();
            playerMovement.SetCanMove(!isDialogueActive);
        }
    }

    void InteractWithNPC()
    {
        if (currentNPC != null)
        {
            // NPC 방향 변경 (플레이어를 향하도록)
            Vector2 direction = (transform.position - currentNPC.transform.position).normalized;
            currentNPC.FaceDirection(direction);
            
            // 플레이어 이동 정지
            if (playerMovement != null)
            {
                playerMovement.SetCanMove(false);
            }
            
            // 대화 시작
            DialogueManager.Instance.StartDialogue(currentNPC.dialogueData);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        NPC npc = other.GetComponent<NPC>();
        if (npc != null)
        {
            currentNPC = npc;
            canInteract = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        NPC npc = other.GetComponent<NPC>();
        if (npc != null && npc == currentNPC)
        {
            currentNPC = null;
            canInteract = false;
        }
    }
}