using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * 카드를 담는 위아래 공간
 */
public class Arranger : MonoBehaviour {
    List<Transform> childCardList;

    void Start() {
        //childCardList 초기화
        childCardList = new List<Transform>();
        UpdateArranger();
    }


    void Update() {

    }

    //어렌져의 카드 순서가 달라질 수 있는경우 업데이트
    public void UpdateArranger() {
        //
        for (int i = 0; i < transform.childCount; i++) {
            //기존 childCardList의 사이즈가 작다면 늘려준다
            if (i == childCardList.Count) {
                childCardList.Add(null);
            }

            // index순서대로 card를 받는다
            var child = transform.GetChild(i);

            //기존 childCardList와 transform.child(i)가 다르다면 최신화한다
            if (child != childCardList[i]) {
                childCardList[i] = child;
            }
        }
        // 리스트에 실존하는 카드를 제외한 필요없는 카드가 삭제된다.
        childCardList.RemoveRange(transform.childCount, childCardList.Count - transform.childCount);

    }

    //
    public void InsertCard(Transform card, int index) {

        childCardList.Add(card);
        //Debug.Log("카드 삽입");

        card.SetSiblingIndex(index);
        //Debug.Log("카드의 인덱스 지정");

        UpdateArranger();
        //Debug.Log("어렌져 Updated");
    }

    // 카드 위치가 사이에 있을때 인덱스를 가져온다. invisible을 무시한채
    public int GetIndexByPosition(Transform card,int skipIndex = -1) {

        int result = 0;

        for(int i = 0; i < childCardList.Count; i++) { 
            if(card.position.x < childCardList[i].position.x){
                break;
            }
            else if(skipIndex != i) {
                result++;
            }
        }

        return result;
    }

    public void SwapCard(int index1, int index2) {
        Central.SwapCards( childCardList[index1], childCardList[index2] ); //static이라 인스턴스 없이 사용 가능
        UpdateArranger();

    }
}
