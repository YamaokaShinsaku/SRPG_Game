using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleWindowUI : MonoBehaviour
{
    /// �o�g�����ʕ\���E�B���h�E ///
    public Text nameText;
    public Image hpGageImg;     // HP�Q�[�W�摜
    public Text hpText;         // HP�e�L�X�g
    public Text damageText;     // �_���[�W�e�L�X�g


    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// �o�g�����ʃE�B���h�E��\������
    /// </summary>
    /// <param name="charaData">�U�����ꂽ�L�����N�^�[�̃f�[�^</param>
    /// <param name="damegeValue">�_���[�W��</param>
    public void ShowWindow(Character.Character charaData, int damegeValue)
    {
        // �I�u�W�F�N�g�̃A�N�e�B�u��
        this.gameObject.SetActive(true);

        // ���O�e�L�X�g�̕\��
        nameText.text = charaData.characterName;

        // �_���[�W���v�Z���āA�c��HP���擾
        int nowHP = charaData.nowHP - damegeValue;
        // HP��0�`�ő�l�͈̔͂Ɏ��܂�悤�ɕ␳
        nowHP = Mathf.Clamp(nowHP, 0, charaData.maxHP);

        // HP�Q�[�W�\��
        float amount = (float)charaData.nowHP / charaData.maxHP;       // �\������ FillAmount
        float endAmount = (float)nowHP / charaData.maxHP;    // �A�j���[�V������� FillAmount

        // HP�Q�[�W�����X�Ɍ���������A�j���[�V����
        DOTween.To(
            () => amount, (n) => amount = n,        // �ω�������ϐ����w��
            endAmount,          // �ω���̐��l
            1.0f)               // �A�j���[�V��������(�b)
            .OnUpdate(() =>     // �A�j���[�V���������t���[�����s����鏈��
            {
               // �ő�l�ɑ΂��錻�݂�HP�̊������摜�� FillAmount �ɃZ�b�g����
               hpGageImg.fillAmount = amount;
           });

        // �e�L�X�g�\��
        hpText.text = nowHP + "/" + charaData.maxHP;
        if(damegeValue >= 0)
        {
            damageText.text = damegeValue + "�_���[�W�I";
        }
        // HP�񕜎�
        else
        {
            damageText.text = -damegeValue + "�񕜁I";
        }
    }

    /// <summary>
    /// �o�g�����ʃE�B���h�E���B��
    /// </summary>
    public void HideWindow()
    {
        this.gameObject.SetActive(false);
    }
}
