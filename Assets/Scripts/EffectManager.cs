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
        [SerializeField]
        private GameObject damageEffect;        // ダメージエフェクト

        public GameObject Target;
        public TextMesh damageText;
        private Vector3 offset;

        // Start is called before the first frame update
        void Start()
        {
            backAttackEffect.SetActive(false);
            dethEffect.SetActive(false);
            damageEffect.SetActive(false);
        }

        /// <summary>
        /// バックアタックエフェクトの再生
        /// </summary>
        /// <param name="selectintCharacter">選択中のキャラクター</param>
        public void PlayBackAttackEffect(Character.Character selectingCharacter)
        {
            // オフセットを設定
            if (selectingCharacter.zPos >= 0.0f)
            {
                offset = new Vector3(-0.5f, 3.0f, -3.0f);
            }
            else if(selectingCharacter.zPos < 0.0f)
            {
                offset = new Vector3(-0.5f, 3.0f, 0.0f);
            }

            // 角度を調整（Targetのほうを向くように）
            Vector3 dir = Target.transform.position - transform.position;
            Quaternion Rotation = Quaternion.LookRotation(dir);

            // エフェクトの表示座標を設定
            backAttackEffect.transform.rotation = Rotation;
            backAttackEffect.transform.position = selectingCharacter.transform.position + offset;

            // エフェクトを再生
            backAttackEffect.SetActive(true);
        }

        /// <summary>
        /// 死亡時のエフェクト再生
        /// </summary>
        /// <param name="selectingCharacter">選択中のキャラクター</param>
        public void PlayDethEffect(Character.Character selectingCharacter)
        {
            GameObject clone = Instantiate(dethEffect,selectingCharacter.transform.parent);
            // オフセットを設定
            offset = new Vector3(0.0f, 0.5f, 0.0f);
            // エフェクトの表示座標を設定
            clone.transform.localPosition = selectingCharacter.transform.position + offset;

            // エフェクトを再生
            clone.SetActive(true);
        }

        /// <summary>
        /// ダメージ数値エフェクトの再生
        /// </summary>
        /// <param name="damageCharacter">ダメージを受けたキャラクター</param>
        /// <param name="damage">ダメージ量</param>
        public void PlayDamageEffect(Character.Character damageCharacter, float damage)
        {
            // オフセットを設定
            if (damageCharacter.zPos >= 0.0f)
            {
                offset = new Vector3(0.0f, 1.0f, 0.0f);
            }
            else if (damageCharacter.zPos < 0.0f)
            {
                offset = new Vector3(0.0f, 1.0f, 0.0f);
            }

            // 角度を調整（Targetのほうを向くように）
            Vector3 dir = Target.transform.position - transform.position;
            Quaternion Rotation = Quaternion.LookRotation(dir);

            // エフェクトの表示座標を設定
            damageEffect.transform.rotation = Rotation;
            damageEffect.transform.position = damageCharacter.transform.position + offset;

            // damageTextにダメージの数値を代入
            damageText.text = damage.ToString();

            // エフェクトを再生
            damageEffect.SetActive(true);
        }
    }
}

