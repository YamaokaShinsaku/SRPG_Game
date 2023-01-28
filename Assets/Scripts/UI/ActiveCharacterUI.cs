using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIManager
{
    public class ActiveCharacterUI : MonoBehaviour
    {
        public Text[] text;

        public ActionCharactor actionCharactor;
        // Start is called before the first frame update
        void Start()
        {
            // コンポーネントを取得
            //actionCharactor = GetComponent<ActionCharactor>();
        }

        // Update is called once per frame
        void Update()
        {
            for(int i = 0; i < actionCharactor.activeCharacters.Count; i++)
            {
                text[i].text = actionCharactor.activeCharacters[i].name;
            }
        }
    }
}
