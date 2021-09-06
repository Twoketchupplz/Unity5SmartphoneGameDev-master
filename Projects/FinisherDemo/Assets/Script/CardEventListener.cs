using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardEventListener : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    Transform root;
    void Start() {
        root = transform.root; //가장 상위계층 트랜스폼
    }

    void Update() {
        
    }
    public void OnBeginDrag(PointerEventData eventData) { //여기서 PointerEventData는 마우스나 터치에 대한 이벤트다.
        //드래그가 시작되면 Canvas(root) 이하로 BeginDrag라는 이름을 갖은 모든 함수가 호출됨
        root.BroadcastMessage("BeginDrag",transform, SendMessageOptions.DontRequireReceiver);
    }

    public void OnDrag(PointerEventData eventData) {
        // 드래그 하는동안 마우스 위치에 카드 위치로 지정한다
        // 오브젝트 트랜스폼의 position을 터치한 곳의 위치로
        transform.position = eventData.position;
        root.BroadcastMessage("Drag", transform, SendMessageOptions.DontRequireReceiver);

    }

    public void OnEndDrag(PointerEventData eventData) {
        root.BroadcastMessage("EndDrag", transform, SendMessageOptions.DontRequireReceiver);
    }
}
