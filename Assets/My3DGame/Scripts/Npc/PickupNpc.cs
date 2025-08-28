using UnityEngine;
using TMPro;
using Unity.VisualScripting;

namespace My3DGame
{
    /// <summary>
    /// npc 인터랙티브 기능 구현, 인터렉티브한 npc의 부모 클래스
    /// </summary>
    public class PickupNpc : MonoBehaviour
    {
        #region Variables
        public Npc npc;

        protected PlayerController playerController;    //유저 캐릭터 오브젝트
        [SerializeField]
        protected float interactiveRange = 2f;                  //인터랙티브 기능이 유효한 거리

        //인터렉티브 UI - 마우스를 가져가면 액션 UI가 보인다
        public TextMeshProUGUI actionUI;
        protected string actionText = "Pickup ";
        #endregion

        #region Unity Event Method
        protected virtual void Start()
        {
            playerController = GameObject.FindFirstObjectByType<PlayerController>();
        }

        protected virtual void OnMouseOver()
        {
            //플레이어와의 거리
            float distance = Vector3.Distance(transform.position, playerController.transform.position);
            
            //유효한 거리안에 있으면
            if(distance < interactiveRange)
            {
                ShowActionUI();
            }
            else
            {
                HideActionUI();
            }

            //액션키를 입력 받으면
            if(Input.GetKeyDown(KeyCode.E) && distance < interactiveRange)
            {
                HideActionUI();
                DoAction();
            }
        }

        protected virtual void OnMouseExit()
        {   
            HideActionUI();
        }
        #endregion

        #region Custom Method
        public virtual void ShowActionUI()
        {
            actionUI.gameObject.SetActive(true);
            actionUI.text = actionText + npc.name;
        }

        public virtual void HideActionUI()
        {
            actionUI.gameObject.SetActive(false);
            actionUI.text = "";
        }

        //액션키(e키)입력시 npc의 인터랙티브 액션 실행
        protected virtual void DoAction()
        {
            Debug.Log("npc의 인터랙티브 액션 실행");
        }
        #endregion
    }
}