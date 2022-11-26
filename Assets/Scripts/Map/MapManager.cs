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
        private const int GENERATE_RATIO_GRASS = 90;

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

        // Update is called once per frame
        void Update()
        {

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
                    mapData[i, j].SetSelectionMode(false);
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

            // ���ꂼ��̕����Ɍ������čs���~�܂�܂ł̃u���b�N�f�[�^�����ԂɎ擾���A���X�g�ɒǉ�����
            // X+ ����
            for(int i = baseX + 1; i < MAP_WIDTH; i++)
            {
                if(AddReachableList(results, mapData[i,baseZ]))
                {
                    break;
                }
            }
            // X- ����
            for(int i = baseX - 1; i >= 0; i--)
            {
                if (AddReachableList(results, mapData[i, baseZ]))
                {
                    break;
                }
            }
            // Z+ ����
            for(int j = baseZ + 1; j < MAP_HEIGHT; j++)
            {
                if (AddReachableList(results, mapData[baseX, j]))
                {
                    break;
                }
            }
            // Z- ����
            for(int j = baseZ - 1; j >= 0; j--)
            {
                if (AddReachableList(results, mapData[baseX, j]))
                {
                    break;
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
    }

}