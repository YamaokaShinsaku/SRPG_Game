using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectManager
{
    public class EffectManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject backAttackEffect;    // バックアタックエフェクト
        [SerializeField]
        private GameObject dethEffect;          // 死亡エフェクト

        public GameObject Target;
        private Vector3 offset;

        // Start is called before the first frame update
        void Start()
        {
            backAttackEffect.SetActive(false);
            dethEffect.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// バックアタックエフェクトの再生
        /// </summary>
        /// <param name="selectintCharacter">選択中のキャラクター</param>
        public void PlayBackAttackEffect(Character.Character selectingCharacter)
        {
            // オフセットを設定
            offset = new Vector3(-0.5f, 3.0f, -3.0f);

            // 角度を調整（Targetのほうを向くように）
            Vector3 dir = Target.transform.position - transform.position;
            Quaternion Rotation = Quaternion.LookRotation(dir);

            // エフェクトの表示座標を設定
            backAttackEffect.transform.rotation = Rotation;
            backAttackEffect.transform.position = selectingCharacter.transform.position + offset;

            // エフェクトを再生
            backAttackEffect.SetActive(true);

            //Debug.Log("Effects");
        }

        /// <summary>
        /// 死亡時のエフェクト再生
        /// </summary>
        /// <param name="selectingCharacter">選択中のキャラクター</param>
        public void PlayDethEffect(Character.Character selectingCharacter)
        {
            GameObject clone = Instantiate(dethEffect);
            // オフセットを設定
            offset = new Vector3(0.0f, 0.5f, 0.0f);
            // エフェクトの表示座標を設定
            clone.transform.position = selectingCharacter.transform.position + offset;

            // エフェクトを再生
            clone.SetActive(true);
        }
    }
}

