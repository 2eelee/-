using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필요

public class MapNode : MonoBehaviour
{
    // 노드의 타입을 정의 (드롭다운으로 선택 가능해짐)
    public enum NodeType
    {
        Transfer,   // 씬 이동용
        Blocked     // 이동 불가 (메시지용)
    }

    [Header("노드 설정")]
    public NodeType nodeType; // 여기서 타입 선택

    [Header("이동용 설정 (Transfer일 때만 사용)")]
    public string targetSceneName; // 이동할 씬 이름 (예: Stage2)

    [Header("차단용 설정 (Blocked일 때만 사용)")]
    public string blockMessage; // 띄울 메시지 내용
    public TextMeshProUGUI messageUI;      // 메시지를 보여줄 UI 텍스트 객체 (연결 필요)
    public GameObject messagePanel; // 텍스트 뒤에 배경 패널이 있다면 연결 (선택사항)

    // 플레이어가 노드(Trigger)에 닿았을 때 실행되는 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 닿은 물체가 'Player' 태그를 달고 있는지 확인
        if (collision.CompareTag("Player"))
        {
            if (nodeType == NodeType.Transfer)
            {
                // [이동용] 해당 씬으로 이동
                if (!string.IsNullOrEmpty(targetSceneName))
                {
                    SceneManager.LoadScene(targetSceneName);
                }
                else
                {
                    Debug.LogError("이동할 씬 이름이 비어있습니다!");
                }
            }
            else if (nodeType == NodeType.Blocked)
            {
                // [차단용] 메시지 띄우기
                ShowBlockMessage();
            }
        }
    }

    // 메시지를 보여주는 함수
    void ShowBlockMessage()
    {
        Debug.Log("이동 불가: " + blockMessage); // 콘솔창 확인용

        // UI 텍스트에 메시지 넣기
        if (messageUI != null)
        {
            messageUI.text = blockMessage;
            messageUI.gameObject.SetActive(true); // 텍스트 켜기

            if (messagePanel != null) messagePanel.SetActive(true); // 배경 패널 켜기

            // 2초 뒤에 메시지 끄기 (Coroutine 사용)
            StartCoroutine(HideMessageAfterDelay(2.0f));
        }
    }

    // 2초 뒤에 꺼주는 타이머 기능
    System.Collections.IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (messageUI != null) messageUI.gameObject.SetActive(false);
        if (messagePanel != null) messagePanel.SetActive(false);
    }
}