using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectManager
{
    public class EffectManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject backAttackEffects;    // バックアタックエフェクト

        public GameObject Target;
        private Vector3 offset;

        // Start is called before the first frame update
        void Start()
        {
            backAttackEffects.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// バックアタックエフェクトの再生
        /// </summary>
        /// <param name="selectintCharacter">選択中のキャラクター</param>
        public void PlayBackAttackEffect(Character.Character selectintCharacter)
        {
            // オフセットを設定
            offset = new Vector3(-0.5f, 3.0f, 0.0f);

            // 角度を調整（Targetのほうを向くように）
            Vector3 dir = Target.transform.position - transform.position;
            Quaternion Rotation = Quaternion.LookRotation(dir);

            // エフェクトの表示座標を設定
            backAttackEffects.transform.rotation = Rotation;
            backAttackEffects.transform.position = selectintCharacter.transform.position + offset;

            // エフェクトを再生
            backAttackEffects.SetActive(true);

            //Debug.Log("Effects");
        }
    }
}

