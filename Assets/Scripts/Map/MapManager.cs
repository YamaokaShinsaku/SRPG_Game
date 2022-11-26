using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
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
        // �������W��ݒ�
        Vector3 defaultPosition = new Vector3(0.0f, 0.0f, 0.0f);
        defaultPosition.x = -(MAP_WIDTH / 2);
        defaultPosition.z = -(MAP_HEIGHT / 2);

        // �u���b�N����
        for(int i = 0; i < MAP_WIDTH; i++)
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
                if(rand < GENERATE_RATIO_GRASS)
                {
                    isGrass = true;
                }

                // �I�u�W�F�N�g�̃N���[���𐶐�
                GameObject obj;
                if(isGrass)
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
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
