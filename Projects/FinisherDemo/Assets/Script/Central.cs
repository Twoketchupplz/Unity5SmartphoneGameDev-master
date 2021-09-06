using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Central : MonoBehaviour
{
    public Transform invisibleCard; //Unity 엔진에서 이 항목에 invisibleCard 오브젝트를 넣어준다

    List<Arranger> arrangerList;

    Arranger originArranger;
    int originIndex;
    // Start is called before the first frame update
    void Start()
    {
        //어렌져 리스트 초기화
        arrangerList = new List<Arranger>();

        var arrs = transform.GetComponentsInChildren<Arranger>();

        for(int i = 0; i < arrs.Length; i++) {
            arrangerList.Add(arrs[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //pos가 rt에 있는지 판별
    bool ContainPos(RectTransform rt, Vector2 pos) {
        return RectTransformUtility.RectangleContainsScreenPoint(rt, pos);
    }

    void BeginDrag(Transform card) {
        //  Debug.Log("BeginDrag :" + card.name);
        //SwapCardInHierarchy(invisibleCard, card); //드래그가 시작되면 invisible이 대신 자리를 차지한다

        //시작할때 위치를 기억한다. 나중에 되돌아 가기위해 exception
        originArranger = arrangerList.Find(t => ContainPos(t.transform as RectTransform, card.position)); //현재 카드가 있던 어렌져의 위치
        originIndex = card.GetSiblingIndex();
        //Debug.Log("시작 어렌져 : " + originArranger.name);
        //Debug.Log("시작 인덱스 : " + originIndex);

        //invisible card와 위치 교환
        SwapCardInHierarchy(invisibleCard, card);
    }
    void Drag(Transform card) {
        //어렌져들 중 해당 카드가 들어있는 리스트를 찾는다
        var arrangerWhichTheCardIs = arrangerList.Find(t => ContainPos(t.transform as RectTransform, card.position));
        //Debug.Log("현재 카드가 있는 어렌져 : " + arrangerWhichTheCardIs);

        //현재 드래그 중인 카드의 어렌져가 없다면 invisible 카드도 Arranger에서 나와 Central 하위에 있어야한다
        if (arrangerWhichTheCardIs == null) { //어렌져가 없다면
            //Debug.Log("어렌져 null");
            //인비져블의 부모가 Central이 아니라면 Central이 되야한다.
            bool updateChildren = transform != invisibleCard.parent;
            //invisibleCard의 부모를 Central로 바꾼다.
            invisibleCard.SetParent(transform);
            //Debug.Log("인비져블의 카드 parent를 " + invisibleCard.parent + "로 변경");

            //모든 어렌져를 재정렬한다.
            if (updateChildren) {
                arrangerList.ForEach(t => t.UpdateArranger());
            }
        }
        //카드를 밖으로 뺐다가 다시 안으로 넣으면 parent가 바뀐다. 순서가 바뀐다;; grid에 영향
        else { // 현재 드래그 중인 카드가 어렌져 위에 있다면
            //Debug.Log("어렌져가 null이 아니다");

            //이때 invisible의 parent가 지금 현재 Central parent와 같다면 Arranger로 insert한다
            bool insert = invisibleCard.parent == transform;

            if (insert) {
                //Debug.Log("인비져블 parent가 Central");
                int index = arrangerWhichTheCardIs.GetIndexByPosition(card);

                invisibleCard.SetParent(arrangerWhichTheCardIs.transform);
                //Debug.Log("인비져블.Parent = " + arrangerWhichTheCardIs);

                arrangerWhichTheCardIs.InsertCard(invisibleCard, index);
            }
            //들어가 있다면
            else {
                // 드래그중 카드가 이동한 위치를 출력한다 0, 1, ..
                //Debug.Log(whichArrangerCard.GetIndexByPosition(card, invisibleCard.GetSiblingIndex()));
                //Debug.Log("인비져블.parent != Central");
                int invisibleCardIndex = invisibleCard.GetSiblingIndex();
                int targetIndex = arrangerWhichTheCardIs.GetIndexByPosition(card, invisibleCardIndex);

                // 같지 않으면 해당 어렌저에서 카드를 스왑한다
                if (invisibleCardIndex != targetIndex) {
                    arrangerWhichTheCardIs.SwapCard(invisibleCardIndex, targetIndex);
                    //Debug.Log("인비저블.parent : " + invisibleCard.parent);
                    //Debug.Log("인비저블.index : " + invisibleCardIndex);
                }
            }
        }
    }
    void EndDrag(Transform card) {
        //Debug.Log("EndDrag 시작");
        //드래그가 끝나면 다시 invisible과 card의 위치를 바꾼다

        //SwapCardInHierarchy(invisibleCard, card);

        //이때 invisibleCard의 부모가 Canvas(어렌져 바깥)이라면.. 카드는 돌아간다 원래위치로
        if (invisibleCard.parent == transform) {
            Debug.Log("복귀");
            Debug.Log(originArranger.name);
            //Debug.Log("인비져블.parent == Central");
            //Debug.Log("제자리로 복귀 시작");
            //Debug.Log("Origin 어렌져 : " + originArranger.name);
            //Debug.Log("Origin 인덱스 : " + originIndex);
            card.SetParent(originArranger.transform);
            originArranger.InsertCard(card, originIndex);
            originArranger = null;
            originIndex = -1;
        }
        else {
            Debug.Log("스왑");
            SwapCardInHierarchy(invisibleCard, card);
        }
    }
    // 선택한 카드와 허상의 카드의 위치를 바꿔준다. 허상카드는 드래그 시 어렌저 레이아웃에 대신 올라간다
    // SiblingIndex와 각 카드의 Parent를 교체한다.
    void SwapCardInHierarchy(Transform source, Transform destination) {
        SwapCards(source, destination);

        //스왑이 일어나면 호출하도록 한다.
        arrangerList.ForEach(t => t.UpdateArranger());
    }

    public static void SwapCards(Transform source, Transform destination) {
        Transform sourParent = source.parent;
        Transform destParent = destination.parent;

        int sourIndex = source.GetSiblingIndex();
        int destIndex = destination.GetSiblingIndex();

        source.SetParent(destParent);
        source.SetSiblingIndex(destIndex);

        destination.SetParent(sourParent);
        destination.SetSiblingIndex(sourIndex);
        //Debug.Log(source.name + "와 " + destination.name + "의 위치가 바뀜");
    }


}
