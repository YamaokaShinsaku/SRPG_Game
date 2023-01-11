using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapManager
{
    public class MapManager : MonoBehaviour
    {
        public MapBlock[,] mapData;     // �}�b�v�f�[�^

        public Transform blockParent;       // �u���b�N�̐e�I�u�W�F�N�g��Transform
        public GameObject blockPrefab_Grass;    // ���u���b�N
        public GameObject blockPrefab_Water;    // ����u���b�N

        // �}�b�v�̉����A����
        public const int MAP_WIDTH = 9;
        public const int MAP_HEIGHT = 9;

        // ���u���b�N�����������m��
        [SerializeField]
        private const int GENERATE_RATIO_GRASS = 100;

        // Start is called before the first frame update
        void Start()
        {
            // �}�b�v�f�[�^�̏�����
            mapData = new MapBlock[MAP_WIDTH, MAP_HEIGHT];

            // �������W��ݒ�
            Vector3 defaultPosition = new Vector3(0.0f, 0.0f, 0.0f);
            defaultPosition.x = -(MAP_WIDTH / 2);
            defaultPosition.z = -(MAP_HEIGHT / 2);

            // �u���b�N����
            for (int i = 0; i < MAP_WIDTH; i++)
            {
                for (int j = 0; j < MAP_HEIGHT; j++)
                {
                    // �u���b�N�̏ꏊ������
                    Vector3 position = defaultPosition;
                    position.x += i;
                    position.z += j;

                    // �u���b�N�̎�ނ�����
                    int rand = Random.Range(0, 100);

                    // ���u���b�N����
                    bool isGrass = false;   // ���u���b�N�����t���O
                    // �����l�����u���b�N�����m�����Ⴂ��
                    if (rand < GENERATE_RATIO_GRASS)
                    {
                        isGrass = true;
                    }

                    // �I�u�W�F�N�g�̃N���[���𐶐�
                    GameObject obj;
                    if (isGrass)
                    {
                        // blockParent�̎q��blockPrefab_Grass�𐶐�
                        obj = Instantiate(blockPrefab_Grass, blockParent);
                    }
                    else
                    {
                        // blockParent�̎q��blockPrefab_Water�𐶐�
                        obj = Instantiate(blockPrefab_Water, blockParent);
                    }

                    // ���W�X�V
                    obj.transform.position = position;

                    // �}�b�v�f�[�^�Ƀu���b�N�f�[�^���i�[
                    var mapBlock = obj.GetComponent<MapBlock>();
                    mapData[i, j] = mapBlock;

                    // �u���b�N�f�[�^�ݒ�
                    mapBlock.xPos = (int)position.x;
                    mapBlock.zPos = (int)position.z;
                }
            }
        }

        /// <summary>
        /// �S�ẴI�u�W�F�N�g�̑I����Ԃ���������
        /// </summary>
        public void AllSelectionModeClear()
        {
            for (int i = 0; i < MAP_WIDTH; i++)
            {
                for (int j = 0; j < MAP_HEIGHT; j++)
                {
                    mapData[i, j].SetSelectionMode(MapBlock.Highlight.Off);
                }
            }
        }

        /// <summary>
        /// �n���ꂽ�ʒu����L�����N�^�[�����B�ł���ꏊ�̃u���b�N�����X�g�ɂ��ĕԂ�
        /// </summary>
        /// <param name="xPos">x���W</param>
        /// <param name="zPos">z���W</param>
        /// <returns>�����𖞂����u���b�N�̃��X�g</returns>
        public List<MapBlock> SearchReachableBlocks(int xPos, int zPos)
        {
            // �����𖞂����u���b�N�̃��X�g
            var results = new List<MapBlock>();

            // ��_�ƂȂ�u���b�N�̔z����ԍ�������
            int baseX = -1, baseZ = -1;     // �z����ԍ�
            // ����
            for(int i = 0; i < MAP_WIDTH; i++)
            {
                for(int j = 0; j < MAP_HEIGHT; j++)
                {
                    // �w�肳�ꂽ���W�Ɉ�v����}�b�v�u���b�N������������
                    if((mapData[i,j].xPos == xPos) && (mapData[i, j].zPos == zPos))
                    {
                        // �z����ԍ����擾
                        baseX = i;
                        baseZ = j;

                        // 2�ڂ̃��[�v�𔲂���
                        break;
                    }
                }
                // ���łɔ����ς݂Ȃ��ڂ̃��[�v�𔲂���
                if (baseX != -1)
                {
                    break;
                }
            }

            // �ړ�����L�����N�^�[�̈ړ����@���擾
            var moveType = Character.Character.MoveType.Rook;   // �ړ����@
            var moveChara =                                     // �ړ�����L�����N�^�[
                GetComponent<Character.CharacterManager>().GetCharacterData(xPos, zPos);
            // �ړ�����L�����N�^�[�����݂��鎞
            if(moveChara != null)
            {
                // �L�����N�^�[�f�[�^����ړ����@���擾
                moveType = moveChara.moveType;
            }

            // �L�����N�^�[�̈ړ����@�ɍ��킹�ĈقȂ�����̃u���b�N�f�[�^���擾
            // Rook, Queen
            if(moveType == Character.Character.MoveType.Rook
                || moveType == Character.Character.MoveType.Queen)
            {
                // X+ ����
                for (int i = baseX + 1; i < MAP_WIDTH; i++)
                {
                    if (AddReachableList(results, mapData[i, baseZ]))
                    {
                        break;
                    }
                }
                // X- ����
                for (int i = baseX - 1; i >= 0; i--)
                {
                    if (AddReachableList(results, mapData[i, baseZ]))
                    {
                        break;
                    }
                }
                // Z+ ����
                for (int j = baseZ + 1; j < MAP_HEIGHT; j++)
                {
                    if (AddReachableList(results, mapData[baseX, j]))
                    {
                        break;
                    }
                }
                // Z- ����
                for (int j = baseZ - 1; j >= 0; j--)
                {
                    if (AddReachableList(results, mapData[baseX, j]))
                    {
                        break;
                    }
                }
            }

            // Bishop, Queen
            if(moveType == Character.Character.MoveType.Bishop 
                || moveType == Character.Character.MoveType.Queen)
            {
                // X+Z+ ����
                for (int i = baseX + 1, j = baseZ + 1; i < MAP_WIDTH && j < MAP_HEIGHT; i++, j++)
                {
                    if (AddReachableList(results, mapData[i, j]))
                    {
                        break;
                    }
                }
                // X-Z+ ����
                for (int i = baseX - 1, j = baseZ + 1; i >= 0 && j < MAP_HEIGHT; i--, j++)
                {
                    if (AddReachableList(results, mapData[i, j]))
                    {
                        break;
                    }
                }
                // X+Z- ����
                for (int i = baseX + 1, j = baseZ - 1; i < MAP_WIDTH && j >= 0; i++, j--)
                {
                    if (AddReachableList(results, mapData[i, j]))
                    {
                        break;
                    }
                }
                // X-Z- ����
                for (int i = baseX - 1, j = baseZ - 1; i >= 0 && j >= 0; i--, j--)
                {
                    if (AddReachableList(results, mapData[i, j]))
                    {
                        break;
                    }
                }
            }

            // �����̃u���b�N
            results.Add(mapData[baseX, baseZ]);

            return results;
        }

        /// <summary>
        /// �w�肵���u���b�N�𓞒B�\�u���b�N���X�g�ɒǉ�����
        /// �i�L�����N�^�[���B�u���b�N�����p�j
        /// </summary>
        /// <param name="reachableList">���B�\�u���b�N���X�g</param>
        /// <param name="targetBlock">�w��̃u���b�N</param>
        /// <returns>�s���~�܂肩�ǂ��� �@true : �s���~�܂�</returns>
        private bool AddReachableList(List<MapBlock> reachableList, MapBlock targetBlock)
        {
            // �Ώۂ̃u���b�N���ʍs�s�Ȃ炻�����s���~�܂�Ƃ���
            if(!targetBlock.isPassable)
            {
                return true;
            }

            // �Ώۂ̈ʒu�ɂق��̃L�����N�^�[�����݂��Ă���Ȃ�A
            // ���B�s�\�ɂ��ďI��
            var charaData =
                GetComponent<Character.CharacterManager>().GetCharacterData(targetBlock.xPos, targetBlock.zPos);
            if(charaData != null)
            {
                return false;
            }

            // ���B�\�u���b�N���X�g�ɒǉ�����
            reachableList.Add(targetBlock);

            return false;
        }

        /// <summary>
        /// �w��ʒu����U���ł���ꏊ�̃}�b�v�u���b�N�����X�g�ɂ��ĕԂ�
        /// </summary>
        /// <param name="xPos">��_x���W</param>
        /// <param name="zPos">��_z���W</param>
        /// <returns>�U���ł���u���b�N�̃��X�g</returns>
        public List<MapBlock> SearchAttackableBlocks(int xPos, int zPos)
        {
            // �����𖞂����}�b�v�u���b�N�̃��X�g
            var results = new List<MapBlock>();

            // ��_�ƂȂ�u���b�N�̔z����ԍ�������
            int baseX = -1, baseZ = -1;
            for(int i = 0; i < MAP_WIDTH; i++)
            {
                for(int j = 0; j < MAP_HEIGHT; j++)
                {
                    if((mapData[i,j].xPos == xPos) && (mapData[i, j].zPos == zPos))
                    {
                        // ��v����}�b�v�u���b�N������Δz����ԍ����擾
                        baseX = i;
                        baseZ = j;

                        // 2�ڂ̃��[�v�𔲂���
                        break;
                    }
                }
                // ���łɔ����ς݂Ȃ�
                if (baseX != -1)
                {
                    // 1�ڂ̃��[�v�𔲂���
                    break;
                }
            }

            // 4������1�}�X�i�񂾈�̃u���b�N���Z�b�g
            // �c��
            // X+����
            AddAttackableList(results, baseX + 1, baseZ);
            // X-����
            AddAttackableList(results, baseX - 1, baseZ);
            // Z+����
            AddAttackableList(results, baseX, baseZ + 1);
            // Z-����
            AddAttackableList(results, baseX, baseZ - 1);
            // �΂�
            // X+Z+����
            AddAttackableList(results, baseX + 1, baseZ + 1);
            // X-Z+����
            AddAttackableList(results, baseX - 1, baseZ + 1);
            // X+Z-����
            AddAttackableList(results, baseX + 1, baseZ - 1);
            // X-Z-����
            AddAttackableList(results, baseX - 1, baseZ - 1);

            return results;
        }

        /// <summary>
        /// �}�b�v�f�[�^�̎w�肳�ꂽ�z����ԍ��ɑΉ�����u���b�N��
        /// �U���\�u���b�N���X�g�ɒǉ�����
        /// </summary>
        /// <param name="attackableList">�U���\�u���b�N���X�g</param>
        /// <param name="indexX">X�����̔z����ԍ�</param>
        /// <param name="indexZ">Z�����̔z����ԍ�</param>
        public void AddAttackableList(List<MapBlock> attackableList, int indexX, int indexZ)
        {
            // �w��ԍ����z��̊O�ɏo�Ă�����ǉ������ɏI��
            if (indexX < 0 || indexX >= MAP_WIDTH || 
                indexZ < 0 || indexZ >= MAP_HEIGHT)
            {
                return;
            }

            // �U���\�u���b�N���X�g�ɒǉ�����
            attackableList.Add(mapData[indexX, indexZ]);
        }

        /// <summary>
        /// �}�b�v�f�[�^�z������X�g�ɂ��ĕԂ�
        /// </summary>
        /// <returns>�}�b�v�f�[�^�̃��X�g</returns>
        public List<MapBlock> MapBlocksToList()
        {
            // ���ʗp���X�g
            var results = new List<MapBlock>();

            // �}�b�v�f�[�^�z��̒��g�����ԂɃ��X�g�Ɋi�[
            for(int i = 0; i < MAP_WIDTH; i++)
            {
                for(int j = 0; j < MAP_HEIGHT; j++)
                {
                    results.Add(mapData[i, j]);
                }
            }

            return results;
        }

    }

}