using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class APBattleManager : MonoBehaviour
{
    [SerializeField]
    private MapManager.MapManager mapManager;
    [SerializeField]
    private Character.CharacterManager characterManager;
    [SerializeField]
    private UIManager.UIManager uiManager;

    // �i�s�Ǘ��p�ϐ�
    public Character.Character selectingCharacter;       // �I�𒆂̃L�����N�^�[
    private Character.SkillDefine.Skill selectingSkill;   // �I�𒆂̃X�L���i�ʏ�U����NONE�Œ�j
    private List<MapBlock> reachableBlocks;               // �I�𒆂̃L�����N�^�[�̈ړ��\�u���b�N���X�g
    private List<MapBlock> attackableBlocks;              // �I�𒆂̃L�����N�^�[�̍U���\�u���b�N���X�g

    public List<Character.Character> activeCharacters;     // isActive��true�ɂȂ��Ă���L�����N�^�[�̃��X�g
    public List<Character.Character> enemyList;            // isEnemy��true�ɂȂ��Ă���L�����N�^�[�̃��X�g

    // �s���L�����Z�������p�ϐ�
    private MapBlock charaAttackBlock;        // �I���L�����N�^�[�̍U����̃u���b�N
    private int charaStartPositionX;          // �I���L�����N�^�[��X���W
    private int charaStartPositionZ;          // �I���L�����N�^�[��Z���W

    [SerializeField]
    private bool isFinish;      // �Q�[���I���t���O

    // �x������
    const float delayTime = 0.5f;

    // �^�[���i�s���[�h
    private enum Phase
    {
        C_Start,             // �A�N�e�B�u�L�����N�^�[�I���t�F�[�Y
        C_SelectDirection,   // �L�����N�^�[�̌�����I��
        MyTurn_Start,        // �����̃^�[���F�J�n
        MyTurn_Moving,       // �����̃^�[���F�ړ���I��
        MyTurn_Command,      // �����̃^�[���F�R�}���h�I��
        MyTurn_Targeting,    // �����̃^�[���F�U���ΏۑI��
        MyTurn_Result,       // �����̃^�[���F���ʕ\��
        Enemyturn_Start,     // �G�̃^�[��  �F�J�n
        EnemyTurn_Result     // �G�̃^�[��  �F���ʕ\��
    }
    private Phase nowPhase;     // ���݂̐i�s���[�h

    // Start is called before the first frame update
    void Start()
    {
        // �R���|�[�l���g���擾
        mapManager = GetComponent<MapManager.MapManager>();
        characterManager = GetComponent<Character.CharacterManager>();
        uiManager = GetComponent<UIManager.UIManager>();

        // ���X�g��������
        reachableBlocks = new List<MapBlock>();
        attackableBlocks = new List<MapBlock>();
        activeCharacters = new List<Character.Character>();

        // �J�n���̐i�s���[�h
        nowPhase = Phase.C_Start;

        // �t�F�[�h�A�E�g�J�n
        uiManager.StartFadeOut();

        // �ŏ��̃L�����N�^�[���Z�b�g
        SetFirstActionCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        // �Q�[�����I�����Ă���Ȃ�
        if (isFinish)
        {
            return;
        }

        // �����I���t�F�[�Y�̂Ƃ�
        if(nowPhase == Phase.C_SelectDirection)
        {
            // ���L�[�̕���������
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectingCharacter.direction = Character.Character.Direction.Forward;
                ChangePhase(Phase.C_Start);
                // �I�u�W�F�N�g���擾
                GetMapObjects();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectingCharacter.direction = Character.Character.Direction.Backward;
                ChangePhase(Phase.C_Start);
                // �I�u�W�F�N�g���擾
                GetMapObjects();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectingCharacter.direction = Character.Character.Direction.Left;
                ChangePhase(Phase.C_Start);
                // �I�u�W�F�N�g���擾
                GetMapObjects();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectingCharacter.direction = Character.Character.Direction.Right;
                ChangePhase(Phase.C_Start);
                // �I�u�W�F�N�g���擾
                GetMapObjects();
            }
        }

        // �^�b�v������o
        if (Input.GetMouseButtonDown(0)
            // UI�ւ̃^�b�v�����o����
            && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            // �f�o�b�O�p����
            // �o�g�����ʃE�B���h�E���\������Ă���Ƃ�
            if (uiManager.battleWindowUI.gameObject.activeInHierarchy)
            {
                // �o�g�����ʃE�B���h�E�����
                uiManager.battleWindowUI.HideWindow();

                // �i�s���[�h��i�߂�
                ChangePhase(Phase.C_Start);

                return;
            }
            // �I�u�W�F�N�g���擾
            GetMapObjects();
        }
    }

    /// <summary>
    /// �I�������L�����N�^�[�̃X�e�[�^�X��\��
    /// </summary>
    /// <param name="targetObject"></param>
    public void OpenStatus(MapBlock targetObject)
    {
        // �L�����N�^�[�f�[�^������
        foreach (Character.Character charaData in characterManager.characters)
        {
            // �L�����N�^�[�̈ʒu���w��̈ʒu�ƈ�v���Ă��邩���`�F�b�N
            if ((charaData.xPos == targetObject.xPos) && (charaData.zPos == targetObject.zPos))
            {
                // �^�b�v�������W�ɂ���L�����N�^�[��UI��\��
                charaData.image.texture = charaData.texture;
                uiManager.ShowCharaStatus(charaData);
                if (charaData.isEnemy)
                {
                    uiManager.ShowEnemyStatusWindow(charaData);
                }
                else
                {
                    uiManager.ShowPlayerStatusWindow(charaData);
                }
            }
        }
    }

    /// <summary>
    /// isActive��true�ɂȂ��Ă���L�����N�^�[���擾
    /// </summary>
    public void SetFirstActionCharacter()
    {
        // isActive��true�ȃL�����N�^�[�̃��X�g���쐬
        if (activeCharacters.Count == 0)
        {
            //activeCharacters = new List<Character.Character>();
            foreach (Character.Character activeCharaData in characterManager.characters)
            {
                // �S�����L�����N�^�[����AisActive�t���O��true�̃L�����N�^�[�����X�g�ɒǉ�
                if (activeCharaData.isActive)
                {
                    activeCharacters.Add(activeCharaData);
                }
            }
        }
    }

    /// <summary>
    /// �Q�[���I�������𖞂����Ă��邩�m�F���A�������Ă���ΏI������
    /// </summary>
    public void CheckFinish()
    {
        // �v���C���[�����t���O
        bool isWin = true;
        // �v���C���[�s�k�t���O
        bool isLose = true;

        // �����Ă��閡���A�G�����݂��邩���`�F�b�N
        foreach (var charaData in characterManager.characters)
        {
            // �G������Ƃ�
            if (charaData.isEnemy)
            {
                // �����t���O��false
                isWin = false;
            }
            // ����������Ƃ�
            else
            {
                // �s�k�t���O��false
                isLose = false;
            }
        }

        // �����A�s�k�t���O�̂ǂ��炩��true�Ȃ�Q�[�����I��
        if (isWin || isLose)
        {
            // �Q�[���I���t���O��true
            isFinish = true;

            // ���S�摜��\��
            DOVirtual.DelayedCall(1.5f, () =>
            {
                // �Q�[���N���A�̎�
                if (isWin)
                {
                    uiManager.ShowGameClearLogo();
                }
                // �Q�[���I�[�o�[�̎�
                else
                {
                    uiManager.ShowGameOverLogo();
                }
                // �t�F�[�h�C���J�n
                uiManager.StartFadeIn();
            });

            // EnhanceScene�̓ǂݍ���
            DOVirtual.DelayedCall(7.0f, () =>
            {
                SceneManager.LoadScene("Enhance");
            });
        }
    }

    /// <summary>
    /// �^�b�v�����ꏊ�̃I�u�W�F�N�g���擾
    /// </summary>
    private void GetMapObjects()
    {
        GameObject targetObject = null;     // �^�b�v�����I�u�W�F�N�g

        // �^�b�v���������ɃJ��������Ray���΂�
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        // Ray�̓�����ʒu�ɑ��݂���I�u�W�F�N�g���擾
        if (Physics.Raycast(ray, out hit))
        {
            targetObject = hit.collider.gameObject;
        }

        // �ΏۃI�u�W�F�N�g�����݂���ꍇ�̏���
        if (targetObject != null)
        {
            SelectBlock(targetObject.GetComponent<MapBlock>());
            OpenStatus(targetObject.GetComponent<MapBlock>());
        }
    }

    /// <summary>
    /// �w�肵���u���b�N��I����Ԃɂ���
    /// </summary>
    /// <param name="targetBlock">�Ώۂ̃I�u�W�F�N�g�f�[�^</param>
    private void SelectBlock(MapBlock targetObject)
    {
        // �i�s���[�h���Ƃɏ����𕪊򂷂�
        switch (nowPhase)
        {
            // �s������L�����N�^�[���G�l�~�[���ǂ����𔻒�
            case Phase.C_Start:
                mapManager.AllSelectionModeClear();
                uiManager.CutinDelete();
                uiManager.HideDirectionText();
                // isActive��true�ȃL�����N�^�[�̃��X�g���쐬
                foreach (Character.Character activeCharaData in characterManager.characters)
                {
                    activeCharaData.activePoint++;
                    if (activeCharaData.activePoint >= 3)
                    {
                        activeCharaData.isActive = true;
                        activeCharaData.activePoint = 3;
                    }

                    if(activeCharaData.activePoint < 3)
                    {
                        activeCharaData.activePoint += 3;
                    }

                    // �S�����L�����N�^�[����AisActive�t���O��true�̃L�����N�^�[�����X�g�ɒǉ�
                    if (activeCharaData.isActive)
                    {
                        if(!activeCharacters.Contains(activeCharaData))
                        {
                            activeCharacters.Add(activeCharaData);
                        }
                    }
                }

                // activeCharacters�̐擪�̃L�����N�^�[��I�𒆂̃L�����N�^�[���ɋL��
                selectingCharacter = activeCharacters[0];
                // �I�𒆂̃L�����N�^�[�̌��݈ʒu���L��
                charaStartPositionX = selectingCharacter.xPos;
                charaStartPositionZ = selectingCharacter.zPos;

                // �L�����N�^�[�̃X�e�[�^�XUI��\������
                //selectingCharacter.statusUI.SetActive(true);
                selectingCharacter.image.texture = selectingCharacter.texture;
                uiManager.SetTexture(selectingCharacter.texture);
                uiManager.ShowCharaStatus(selectingCharacter);
                selectingCharacter.selectingObj.SetActive(true);
                if(selectingCharacter.isEnemy)
                {
                    uiManager.ShowEnemyStatusWindow(selectingCharacter);
                }
                else
                {
                    uiManager.ShowPlayerStatusWindow(selectingCharacter);
                }
                // �ړ��\�ȏꏊ���X�g���擾����
                reachableBlocks =
                    mapManager.SearchReachableBlocks(selectingCharacter.xPos, selectingCharacter.zPos);
                // �ړ��\�ȏꏊ���X�g��\������
                foreach (MapBlock mapBlock in reachableBlocks)
                {
                    mapBlock.SetSelectionMode(MapBlock.Highlight.Reachable);
                }

                // �I�������L�����N�^�[���G�l�~�[�̎�
                if (selectingCharacter.isEnemy)
                {
                    if(!enemyList.Contains(selectingCharacter))
                    {
                        enemyList.Add(selectingCharacter);
                    }
                    ChangePhase(Phase.Enemyturn_Start);
                }
                // �v���C���[�L�����̎�
                else
                {
                    ChangePhase(Phase.MyTurn_Start);
                }

                break;
            // �����̃^�[�� �F �J�n
            case Phase.MyTurn_Start:
                // �S�u���b�N�̑I����Ԃ���������
                mapManager.AllSelectionModeClear();
                // �u���b�N��I����Ԃɂ���
                targetObject.SetSelectionMode(MapBlock.Highlight.Select);

                Debug.Log("�I�u�W�F�N�g���^�b�v����܂��� \n�u���b�N���W : "
                    + targetObject.transform.position);

                // �L�����N�^�[�̃X�e�[�^�XUI��\������
                if (selectingCharacter.isEnemy)
                {
                    uiManager.ShowEnemyStatusWindow(selectingCharacter);
                }
                else
                {
                    uiManager.ShowPlayerStatusWindow(selectingCharacter);
                }

                // �ړ��\�ȏꏊ���X�g���擾����
                reachableBlocks =
                    mapManager.SearchReachableBlocks(selectingCharacter.xPos, selectingCharacter.zPos);
                // �ړ��\�ȏꏊ���X�g��\������
                foreach (MapBlock mapBlock in reachableBlocks)
                {
                    mapBlock.SetSelectionMode(MapBlock.Highlight.Reachable);
                }

                // �ړ��L�����Z���{�^����\��
                uiManager.ShowMoveCancelButton();
                // �i�s���[�h��i�߂�
                ChangePhase(Phase.MyTurn_Moving); 
 
                break;
            // �����̃^�[�� : �ړ�
            case Phase.MyTurn_Moving:
                // �G�L�����N�^�[��I�𒆂Ȃ�ړ����L�����Z�����ďI��
                if (selectingCharacter.isEnemy)
                {
                    CancelMoving();
                    break;
                }

                // �I�������u���b�N���ړ��\�ȏꏊ���X�g���ɂ���Ƃ�
                if (reachableBlocks.Contains(targetObject))
                {
                    // �I�𒆂̃L�����N�^�[���ړ�������
                    selectingCharacter.MovePosition(targetObject.xPos, targetObject.zPos);
                    selectingCharacter.animation.SetBool("WalkFlag", true);

                    // �ړ��\�ȏꏊ���X�g������������
                    reachableBlocks.Clear();
                    // �S�u���b�N�̑I����Ԃ�����
                    mapManager.AllSelectionModeClear();
                    // �ړ��L�����Z���{�^�����\����
                    uiManager.HideMoveCancelButton();

                    // �w�莞�Ԍ�ɏ��������s����
                    DOVirtual.DelayedCall(delayTime, () =>
                    {
                        // �x�����s������e
                        // �R�}���h�{�^����\������
                        uiManager.ShowCommandButtons(selectingCharacter);
                        selectingCharacter.animation.SetBool("WalkFlag", false);

                        // �i�s���[�h��i�߂�
                        ChangePhase(Phase.MyTurn_Command);
                    });
                }
                break;
            // �����̃^�[�� : �R�}���h�I��
            case Phase.MyTurn_Command:
                // �U���͈͂̃u���b�N��I�������Ƃ��A�s�����邩�ǂ����̃{�^����\��
                if (attackableBlocks.Contains(targetObject))
                {
                    // �U����̃u���b�N�����L��
                    charaAttackBlock = targetObject;
                    // �s������E�L�����Z���{�^����\������
                    uiManager.ShowDecideButtons();

                    // �U���\�ȏꏊ���X�g��������
                    attackableBlocks.Clear();
                    // �S�u���b�N�̑I����Ԃ�����
                    mapManager.AllSelectionModeClear();

                    // �U����̃u���b�N�������\��
                    charaAttackBlock.SetSelectionMode(MapBlock.Highlight.Attackable);

                    // �U���Ώۂ̈ʒu�ɂ���L�����N�^�[�̃f�[�^���擾
                    var targetChara =
                        characterManager.GetCharacterData(charaAttackBlock.xPos, charaAttackBlock.zPos);

                    if(targetChara != null)
                    {
                        //targetChara.statusUI.SetActive(true);
                        uiManager.ShowCharaStatus(targetChara);

                    }

                    // �i�s���[�h��i�߂�
                    ChangePhase(Phase.MyTurn_Targeting);
                }
                break;
        }
    }

    /// <summary>
    /// �^�[���i�s���[�h��ύX
    /// </summary>
    /// <param name="newPhase">�ύX��̃��[�h</param>
    /// <param name="noLogos">���S��\���t���O</param>
    private void ChangePhase(Phase newPhase, bool noLogos = false)
    {
        // �Q�[�����I�����Ă���Ȃ�
        if (isFinish)
        {
            return;
        }

        // �i�s���[�h��ύX
        nowPhase = newPhase;

        // ����̃��[�h�ɐ؂�ւ�����^�C�~���O�ōs������
        switch (nowPhase)
        {
            // �����̃^�[�� : �J�n
            case Phase.MyTurn_Start:
                if (!noLogos)
                {
                    // ���S�摜��\��
                    uiManager.ShowPlayerTurnLogo();
                }
                break;
            // �G�̃^�[�� : �J�n
            case Phase.Enemyturn_Start:
                if (!noLogos)
                {
                    // ���S�摜��\��
                    uiManager.ShowEnemyTurnLogo();
                }

                // �G�̍s���������J�n����(�x�����s)
                DOVirtual.DelayedCall(delayTime, () =>
                {
                    EnemyCommand();
                });
                break;
            case Phase.C_SelectDirection:
                uiManager.ShowDirectionText();
                Debug.Log("SelectDirectionPhase");
                break;
        }
    }

    /// <summary>
    /// �I�𒆂̃L�����N�^�[��񏉊�������
    /// </summary>
    private void ClearSelectingChara()
    {
        // �I�𒆂̃L�����N�^�[������������
        selectingCharacter = null;
        // �L�����N�^�[�̃X�e�[�^�X��UI���\���ɂ���
        uiManager.HidePlayerStatusWindow();
        uiManager.HideEnemyStatusWindow();
    }

    /// <summary>
    /// �U���R�}���h�{�^������
    /// </summary>
    public void AttackCommand()
    {
        // �X�L���̑I����Ԃ��I�t�ɂ���
        selectingSkill = Character.SkillDefine.Skill.None;
        // �U���͈͂��擾���ĕ\��
        GetAttackableBlocks();
    }

    /// <summary>
    /// �X�L���R�}���h�{�^������
    /// </summary>
    public void SkillCommand()
    {
        // �L�����N�^�[�̎��X�L����I����Ԃɂ���
        selectingSkill = selectingCharacter.skill;
        // �U���͈͂��擾���ĕ\��
        GetAttackableBlocks();
    }

    /// <summary>
    /// �U���E�X�L���R�}���h�I�����ɑΏۃu���b�N��\������
    /// </summary>
    private void GetAttackableBlocks()
    {
        // �R�}���h�{�^�����\���ɂ���
        uiManager.HideCommandButtons();
        // �U���\�ȏꏊ���X�g���擾����
        // �X�L�� : �t�@�C�A�{�[���̏ꍇ�̓}�b�v�S��ɑΉ�
        if (selectingSkill == Character.SkillDefine.Skill.FireBall)
        {
            attackableBlocks = mapManager.MapBlocksToList();
        }
        else
        {
            attackableBlocks =
                mapManager.SearchAttackableBlocks(selectingCharacter.xPos, selectingCharacter.zPos);
        }

        // �U���\�ȏꏊ���X�g��\��
        foreach (MapBlock mapBlock in attackableBlocks)
        {
            mapBlock.SetSelectionMode(MapBlock.Highlight.Attackable);
        }
    }

    /// <summary>
    /// �ҋ@����
    /// </summary>
    public void StandCommand()
    {
        // �R�}���h�{�^�����\����
        uiManager.HideCommandButtons();

        //selectingCharacter.statusUI.SetActive(false);
        uiManager.HideCharaStatus(selectingCharacter);
        selectingCharacter.texture.Release();
        selectingCharacter.selectingObj.SetActive(false);
        uiManager.HidePlayerStatusWindow();
        uiManager.HideEnemyStatusWindow();
        // �I�𒆂̃L�����N�^�[�����X�g����폜
        selectingCharacter.isActive = false;
        activeCharacters.RemoveAt(0);

        selectingCharacter.activePoint--;
        foreach (Character.Character charaData in characterManager.characters)
        {
            // �S�����L�����N�^�[��activePoint�����Z
            charaData.activePoint++;
        }

        // �i�s���[�h��i�߂�
        //ChangePhase(Phase.C_Start);
        ChangePhase(Phase.C_SelectDirection);
    }

    /// <summary>
    /// �I�𒆂̃L�����N�^�[�̈ړ����͑҂���Ԃ���������
    /// </summary>
    public void CancelMoving()
    {
        // �S�u���b�N�̑I����Ԃ�����
        mapManager.AllSelectionModeClear();
        // �ړ��\�ȏꏊ���X�g��������
        reachableBlocks.Clear();
        // �ړ��L�����Z���{�^�����\����
        uiManager.HideMoveCancelButton();
        // �i�s���[�h�����ɖ߂�
        ChangePhase(Phase.MyTurn_Start, true);
    }

    /// <summary>
    /// �U�����̃L�����N�^�[�̌������C��
    /// </summary>
    /// <param name="attackChara">�U������L�����N�^�[</param>
    /// <param name="defenseChara">�U�������L�����N�^�[</param>
    private void ChangeAttackDirection(Character.Character attackChara, Character.Character defenseChara)
    {
        Character.Character.Direction attacker = attackChara.direction;        // �U�����̌���
        Character.Character.Direction defender = defenseChara.direction;       // �h�䑤�̌���

        // �㑤�̃L�����N�^�[���U������Ƃ�
        if (attackChara.zPos < defenseChara.zPos
            &&( attackChara.xPos + 1 == defenseChara.xPos
            || attackChara.xPos == defenseChara.xPos
            || attackChara.xPos - 1 == defenseChara.xPos))
        {
            Debug.Log("Forw :" + attackChara.zPos + " : " + defenseChara.zPos);
            attackChara.direction = Character.Character.Direction.Forward;
        }
        // �����̃L�����N�^�[���U������Ƃ�
        if (attackChara.zPos > defenseChara.zPos
            &&( attackChara.xPos + 1 == defenseChara.xPos
            || attackChara.xPos == defenseChara.xPos
            || attackChara.xPos - 1 == defenseChara.xPos))
        {
            Debug.Log("Back :" + attackChara.zPos + " : " + defenseChara.zPos);
            attackChara.direction = Character.Character.Direction.Backward;
        }

        if (attacker != Character.Character.Direction.Right)
        {
            // �E���̃L�����N�^�[���U������Ƃ�
            if (attackChara.xPos + 1 == defenseChara.xPos
                && attackChara.zPos == defenseChara.zPos)
            {
                attackChara.direction = Character.Character.Direction.Right;
            }
        }
        if (attacker != Character.Character.Direction.Left)
        {
            // �����̃L�����N�^�[���U������Ƃ�
            if (attackChara.xPos - 1 == defenseChara.xPos
                && attackChara.zPos == defenseChara.zPos)
            {
                attackChara.direction = Character.Character.Direction.Left;
            }
        }
    }

    /// <summary>
    /// �U������
    /// </summary>
    /// <param name="attackChara">�U������L�����N�^�[</param>
    /// <param name="defenseChara">�U�������L�����N�^�[</param>
    private void Attack(Character.Character attackChara, Character.Character defenseChara)
    {
        Debug.Log("�U���� : " + attackChara.characterName
            + "  �h�䑤 : " + defenseChara.characterName);

        //attackChara.statusUI.SetActive(true);
        //defenseChara.statusUI.SetActive(true);
        attackChara.image.texture = attackChara.texture;
        defenseChara.image.texture = defenseChara.texture;
        uiManager.rawImg.texture = attackChara.texture;
        uiManager.ShowCharaStatus(attackChara);
        uiManager.ShowCharaStatus(defenseChara);

        if (attackChara.isEnemy)
        {
            uiManager.ShowPlayerStatusWindow(defenseChara);
            uiManager.ShowEnemyStatusWindow(attackChara);
        }
        else
        {
            uiManager.ShowPlayerStatusWindow(attackChara);
            uiManager.ShowEnemyStatusWindow(defenseChara);
        }


        // �_���[�W�v�Z
        int damageValue;    // �_���[�W��
        int attackPoint = attackChara.atk;     // �U�����鑤�̍U����
        int defencePoint = defenseChara.def;   // �U������鑤�̖h���

        // �h��͂O�i�f�o�t�j���������Ă��鎞
        if (defenseChara.isDefBreak)
        {
            defencePoint = 0;
        }

        // �_���[�W�@���@�U���́@�[�@�h���
        damageValue = attackPoint - defencePoint;
        // ���������ɂ��_���[�W�{�����v�Z
        // �_���[�W�{�����擾
        float ratio =
            GetDamageRatioAttribute(attackChara, defenseChara)
            + GetDamageRatioDirection(attackChara, defenseChara);    // �o�b�N�A�^�b�N�܂�
        //float ratio =
        //   GetDamageRatioAttribute(attackChara, defenseChara);    // �����̂�
        damageValue = (int)(damageValue * ratio);       // �{����K��(int�^�ɕϊ�)

        // �_���[�W�ʂ�0�ȉ��̎�
        if (damageValue < 0)
        {
            // 0�ɂ���
            damageValue = 0;
        }

        // �I�������X�L���ɂ��_���[�W�l�␳�ƌ��ʏ���
        switch (selectingSkill)
        {
            //�N���e�B�J���i��S�̈ꌂ�j
            case Character.SkillDefine.Skill.Critical:
                // �_���[�W2�{
                damageValue *= 2;
                // �X�L���g�p�s�\��Ԃ�
                attackChara.isSkillLock = true;
                break;
            // �V�[���h�j��
            case Character.SkillDefine.Skill.DefBreak:
                // �_���[�W0�Œ�
                damageValue = 0;
                // �h��͂O�i�f�o�t�j���Z�b�g
                defenseChara.isDefBreak = true;
                break;
            // �q�[��
            case Character.SkillDefine.Skill.Heal:
                // �񕜁i�񕜗ʂ͍U���͂̔����j
                damageValue = (int)(attackPoint * -0.5f);
                break;
            // �t�@�C�A�{�[��
            case Character.SkillDefine.Skill.FireBall:
                // �^����_���[�W����
                damageValue /= 2;
                break;
            // �X�L������ or �ʏ�U��
            default:
                break;
        }

        // �U���A�j���[�V����
        // �q�[���E�t�@�C�A�{�[���̓A�j���[�V��������
        if (selectingSkill != Character.SkillDefine.Skill.Heal
            && selectingSkill != Character.SkillDefine.Skill.FireBall)
        {
            //uiManager.CutinActive();
            attackChara.AttackAnimation(defenseChara);
            attackChara.animation.SetBool("AttackFlag", true);
            defenseChara.animation.SetBool("DamageFlag", true);
        }
        // �U�������������^�C�~���O��SE���Đ�
        DOVirtual.DelayedCall(0.45f, () =>
        {
            GetComponent<AudioSource>().Play();
            attackChara.animation.SetBool("AttackFlag", false);
            defenseChara.animation.SetBool("DamageFlag", false);
        });

        // �o�g�����ʕ\���E�B���h�E�̕\���ݒ�
        uiManager.battleWindowUI.ShowWindow(defenseChara, damageValue);

        // �_���[�W�ʕ��U�����ꂽ����HP������������
        defenseChara.nowHP -= damageValue;
        // HP��0�`�ő�l�͈̔͂Ɏ��܂�悤�ɕ␳
        defenseChara.nowHP = Mathf.Clamp(defenseChara.nowHP, 0, defenseChara.maxHP);

        // HP��0�ɂȂ����L�����N�^�[���폜����
        if (defenseChara.nowHP == 0)
        {
            characterManager.DeleteCharaData(defenseChara);
            activeCharacters.Remove(defenseChara);
        }

        // �X�L���̑I����Ԃ���������
        selectingSkill = Character.SkillDefine.Skill.None;

        // �^�[���؂�ւ�����
        DOVirtual.DelayedCall(2.0f, () =>
        {
            // �x�����s������e
            // �E�B���h�E���\����
            uiManager.battleWindowUI.HideWindow();
            uiManager.HideCharaStatus(attackChara);
            uiManager.HideCharaStatus(defenseChara);
            // �^�[����؂�ւ���
            if (nowPhase == Phase.MyTurn_Result)
            {
                DOVirtual.DelayedCall(1.0f, () =>
                {
                    //attackChara.statusUI.SetActive(false);
                    //defenseChara.statusUI.SetActive(false);
                    //uiManager.CutinDelete();
                    attackChara.texture.Release();
                    defenseChara.texture.Release();
                    uiManager.HidePlayerStatusWindow();
                    uiManager.HideEnemyStatusWindow();
                });
                attackChara.texture.Release();
                defenseChara.texture.Release();
                selectingCharacter.selectingObj.SetActive(false);
                // �I�𒆂̃L�����N�^�[�����X�g����폜
                selectingCharacter.isActive = false;
                activeCharacters.RemoveAt(0);

                selectingCharacter.activePoint -= 2;

                // �L�����I���t�F�[�Y��
                //ChangePhase(Phase.C_Start);
                ChangePhase(Phase.C_SelectDirection);
            }
            else if (nowPhase == Phase.EnemyTurn_Result)
            {
                attackChara.texture.Release();
                defenseChara.texture.Release();
                uiManager.HidePlayerStatusWindow();
                uiManager.HideEnemyStatusWindow();
                attackChara.texture.Release();
                defenseChara.texture.Release();
                selectingCharacter.selectingObj.SetActive(false);
                // �I�𒆂̃L�����N�^�[�����X�g����폜
                selectingCharacter.isActive = false;
                activeCharacters.RemoveAt(0);
                if(selectingCharacter.isEnemy)
                {
                    enemyList.RemoveAt(0);
                }
                selectingCharacter.activePoint -= 2;

                // �L�����I���t�F�[�Y��
                ChangePhase(Phase.C_Start);
                //ChangePhase(Phase.C_SelectDirection);
            }
        });

    }

    /// <summary>
    /// �s�����e���菈��
    /// </summary>
    public void ActionDecide()
    {
        // �s������E�L�����Z���{�^�����\����
        uiManager.HideDecideButtons();
        // �U����̃u���b�N�̋����\������������
        charaAttackBlock.SetSelectionMode(MapBlock.Highlight.Off);

        // �U���Ώۂ̈ʒu�ɂ���L�����N�^�[�̃f�[�^���擾
        var targetChara =
            characterManager.GetCharacterData(charaAttackBlock.xPos, charaAttackBlock.zPos);

        // �U���Ώۂ̃L�����N�^�[�����݂���Ƃ�
        if(targetChara != null)
        {
            // �L�����N�^�[�U������
            Attack(selectingCharacter, targetChara);
            ChangeAttackDirection(selectingCharacter, targetChara);
            //targetChara.statusUI.SetActive(false);
            //selectingCharacter.statusUI.SetActive(false);
            //uiManager.HideCharaStatus(targetChara);
            //uiManager.HideCharaStatus(selectingCharacter);
            targetChara.texture.Release();
            selectingCharacter.texture.Release();
            selectingCharacter.selectingObj.SetActive(false);
            uiManager.HidePlayerStatusWindow();
            uiManager.HideEnemyStatusWindow();
            // �I�𒆂̃L�����N�^�[�����X�g����폜
            selectingCharacter.isActive = false;
            activeCharacters.RemoveAt(0);

            foreach (Character.Character charaData in characterManager.characters)
            {
                // �S�����L�����N�^�[��activePoint�����Z
                charaData.activePoint++;
            }
            // �i�s���[�h��i�߂�
            ChangePhase(Phase.MyTurn_Result);
        }
        // �U���Ώۂ����݂��Ȃ�
        else
        {
            // �I�𒆂̃L�����N�^�[�����X�g����폜
            selectingCharacter.selectingObj.SetActive(false);
            uiManager.HidePlayerStatusWindow();
            uiManager.HideEnemyStatusWindow();
            targetChara.texture.Release();
            selectingCharacter.texture.Release();
            selectingCharacter.isActive = false;
            activeCharacters.RemoveAt(0);
            selectingCharacter.activePoint--;
            foreach (Character.Character charaData in characterManager.characters)
            {
                // �S�����L�����N�^�[��activePoint�����Z
                charaData.activePoint++;
            }

            // �i�s���[�h��i�߂�
            //ChangePhase(Phase.C_Start);
            ChangePhase(Phase.C_SelectDirection);
        }
    }

    /// <summary>
    /// �s�����e���Z�b�g����
    /// </summary>
    public void ActionCancel()
    {
        // �s������E�L�����Z���{�^�����\����
        uiManager.HideDecideButtons();
        // �U����̃u���b�N�̋����\��������
        charaAttackBlock.SetSelectionMode(MapBlock.Highlight.Off);

        // �L�����N�^�[���ړ��O�̈ʒu�ɖ߂�
        selectingCharacter.MovePosition(charaStartPositionX, charaStartPositionZ);
        // �L�����N�^�[�̑I����Ԃ���������
        //ClearSelectingChara();

        // �i�s���[�h��߂�
        ChangePhase(Phase.MyTurn_Start, true);
    }

    /// <summary>
    /// �G�L�����N�^�[�̂�����̂��s�������ă^�[�����I������
    /// </summary>
    private void EnemyCommand()
    {
        // �X�L���̑I����Ԃ��I�t�ɂ���
        selectingSkill = Character.SkillDefine.Skill.None;

        //selectingCharacter.statusUI.SetActive(true);
        uiManager.ShowCharaStatus(selectingCharacter);

        // �U���\�ȃL�����N�^�[�E�ʒu�̑g�ݍ��킹��1�����_���Ɏ擾
        var actionPlan = TargetFinder.GetRandomactionPlans(mapManager, characterManager, enemyList);
        // �g�ݍ��킹�̃f�[�^�����݂���΁A�U������
        if (actionPlan != null)
        {
            // �ړ�����
            actionPlan.charaData.MovePosition(actionPlan.toMoveBlock.xPos, actionPlan.toMoveBlock.zPos);
            //actionPlan.charaData.animation.SetBool("WalkFlag", true);
            // �U������(�ړ���ɍU���J�n)
            DOVirtual.DelayedCall(delayTime, () =>
            {
                //actionPlan.charaData.animation.SetBool("WalkFlag", false);
                mapManager.AllSelectionModeClear();
                Attack(actionPlan.charaData, actionPlan.toAttaackChara);
                ChangeAttackDirection(actionPlan.charaData, actionPlan.toAttaackChara);
            });

            // �i�s���[�h��i�߂�
            ChangePhase(Phase.EnemyTurn_Result);
            return;
        }

        // �U���\�ȃL�����N�^�[��������Ȃ�������
        // �ړ�������L�����N�^�[����̃����_���ɑI��
        int randID = Random.Range(0, enemyList.Count);
        //  �s������G�L�����N�^�[�̃f�[�^���擾
        Character.Character targetEnemy = enemyList[0];
        // �Ώۂ̈ړ��\�ȏꏊ���X�g�̂Ȃ�����P�̏ꏊ�������_���ɑI��
        reachableBlocks =
            mapManager.SearchReachableBlocks(targetEnemy.xPos, targetEnemy.zPos);
        if (reachableBlocks.Count > 0)
        {
            randID = Random.Range(0, reachableBlocks.Count);
            // �ړ���̃u���b�N�f�[�^
            MapBlock targetBlock = reachableBlocks[randID];
            // �ړ�����
            //targetEnemy.animation.SetBool("AttackFlag", true);
            targetEnemy.MovePosition(targetBlock.xPos, targetBlock.zPos);
            //DOVirtual.DelayedCall(delayTime, () =>
            //{
            //    targetEnemy.animation.SetBool("AttackFlag", false);
            //});
        }

        // ���X�g���N���A
        reachableBlocks.Clear();
        attackableBlocks.Clear();

        mapManager.AllSelectionModeClear();

        // �I�𒆂̃L�����N�^�[�����X�g����폜
        selectingCharacter.selectingObj.SetActive(false);
        //selectingCharacter.statusUI.SetActive(false);
        //targetEnemy.statusUI.SetActive(false);
        //uiManager.HideCharaStatus(selectingCharacter);
        //uiManager.HideCharaStatus(targetEnemy);
        uiManager.HidePlayerStatusWindow();
        uiManager.HideEnemyStatusWindow();
        selectingCharacter.isActive = false;
        activeCharacters.RemoveAt(0);
        enemyList.RemoveAt(0);
        selectingCharacter.activePoint--;
        foreach (Character.Character charaData in characterManager.characters)
        {
            // �S�����L�����N�^�[��activePoint�����Z
            charaData.activePoint++;
        }

        DOVirtual.DelayedCall(delayTime, () =>
        {
            // �i�s���[�h��i�߂�
            ChangePhase(Phase.C_Start);
        });
    }

    /// <summary>
    /// ���������ɂ��_���[�W�{����Ԃ�
    /// </summary>
    /// <param name="attackChara">�U������L�����N�^�[�f�[�^</param>
    /// <param name="defenseChara">�U�������L�����N�^�[�f�[�^</param>
    /// <returns>�_���[�W�{��</returns>
    private float GetDamageRatioAttribute(Character.Character attackChara, Character.Character defenseChara)
    {
        // �_���[�W�{�����`
        const float normal = 1.0f;     // �ʏ�
        const float good = 1.2f;       // �L��
        const float bad = 0.8f;        // �s��

        Character.Character.Attribute attacker = attackChara.attribute;        // �U�����̑���
        Character.Character.Attribute defender = defenseChara.attribute;       // �h�䑤�̑���

        // �������菈��
        // �������ƂɗL�����s���̏��Ń`�F�b�N���A�ǂ���ɂ����Ă͂܂�Ȃ���Βʏ�{����Ԃ�
        switch (attacker)
        {
            // �U���� : ������
            case Character.Character.Attribute.Water:
                if (defender == Character.Character.Attribute.Fire)
                {
                    return good;
                }
                else if (defender == Character.Character.Attribute.Soil)
                {
                    return bad;
                }
                else
                {
                    return normal;
                }
            // �U���� : �Α���
            case Character.Character.Attribute.Fire:
                if (defender == Character.Character.Attribute.Wind)
                {
                    return good;
                }
                else if (defender == Character.Character.Attribute.Water)
                {
                    return bad;
                }
                else
                {
                    return normal;
                }
            // �U���� : ������
            case Character.Character.Attribute.Wind:
                if (defender == Character.Character.Attribute.Soil)
                {
                    return good;
                }
                else if (defender == Character.Character.Attribute.Fire)
                {
                    return bad;
                }
                else
                {
                    return normal;
                }
            // �U���� : �y����
            case Character.Character.Attribute.Soil:
                if (defender == Character.Character.Attribute.Water)
                {
                    return good;
                }
                else if (defender == Character.Character.Attribute.Wind)
                {
                    return bad;
                }
                else
                {
                    return normal;
                }
            // �f�t�H���g�ݒ�
            default:
                return normal;
        }
    }

    /// <summary>
    /// �L�����N�^�[�̌����ɉ������_���[�W��Ԃ�
    /// </summary>
    /// <param name="attackChara">�U������L�����N�^�[�f�[�^</param>
    /// <param name="defenseChara">�U�������L�����N�^�[�f�[�^</param>
    /// <returns>�_���[�W�{��</returns>
    private float GetDamageRatioDirection(Character.Character attackChara, Character.Character defenseChara)
    {
        // �_���[�W�{�����`
        //const float normal = 1.0f;      // �ʏ�
        const float good = 5.8f;        // �o�b�N�A�^�b�N(0.8�{)

        Character.Character.Direction attacker = attackChara.direction;        // �U�����̌���
        Character.Character.Direction defender = defenseChara.direction;       // �h�䑤�̌���

        // �������菈��
        // �������ƂɗL�����s���̏��Ń`�F�b�N���A�o�b�N�A�^�b�N�łȂ����normal��Ԃ�
        switch (defender)
        {
            // �h�䑤 : Forward
            case Character.Character.Direction.Forward:
                if(attackChara.zPos == defenseChara.zPos - 1
                    && attackChara.xPos == defenseChara.xPos)
                {
                    return good;
                }
                else
                {
                    return 0.0f;
                }
            // �h�䑤 : Backward
            case Character.Character.Direction.Backward:
                if(attackChara.zPos == defenseChara.zPos + 1
                    && attackChara.xPos == defenseChara.xPos)
                {
                    return good;
                }
                else
                {
                    return 0.0f;
                }
            // �h�䑤 : Right
            case Character.Character.Direction.Right:
                if(attackChara.xPos == defenseChara.xPos - 1
                    && attackChara.zPos == defenseChara.zPos)
                {
                    return good;
                }
                else
                {
                    return 0.0f;
                }
            // �h�䑤 : Left
            case Character.Character.Direction.Left:
                if (attackChara.xPos == defenseChara.xPos + 1
                    && attackChara.zPos == defenseChara.zPos)
                {
                    return good;
                }
                else
                {
                    return 0.0f;
                }
            // �f�t�H���g�ݒ�
            default:
                return 0.0f;
        }
    }

    /// <summary>
    /// �I�𒆂̃L�����N�^�[�̌�����ύX
    /// </summary>
    public void DirectionForward()
    {
        selectingCharacter.direction = Character.Character.Direction.Forward;
    }
    public void DirectionBackward()
    {
        selectingCharacter.direction = Character.Character.Direction.Backward;
    }
    public void DirectionRight()
    {
        selectingCharacter.direction = Character.Character.Direction.Right;
    }
    public void DirectionLeft()
    {
        selectingCharacter.direction = Character.Character.Direction.Left;
    }
}