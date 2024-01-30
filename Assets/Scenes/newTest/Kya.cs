using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Kya : MonoBehaviour
{

    int count = 0;
    int count2 = 0;

    CancellationTokenSource cts;

    private async void Awake()
    {
        cts = new CancellationTokenSource();

        var _ = TestTask(cts.Token);


    }



    async UniTask TestTask(CancellationToken token)
    {
        var _ = StopTask();
        Debug.Log(count2);
        count2++;
        while (true)
        {
            Debug.Log("Task is running");
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f),false,PlayerLoopTiming.Update, token);
        }
    }

    async UniTask StopTask()
    {
        await UniTask.Delay(2000);
        cts.Cancel();
        cts.Dispose();
        cts = new CancellationTokenSource();
        Debug.Log("Task is stopping");
        Debug.Log(count);
        count++;
        var _ = TestTask(cts.Token);

    }

}



//private CancellationTokenSource _cts;
//private CancellationToken _ct;
//private int num = 0;

//async void Start()
//{
//    Debug.Log("start");
//    //CancellationTokenSource�̃C���X�^���X�𐶐�
//    _cts = new CancellationTokenSource();
//    //_cts��Token�𐶐�
//    _ct = _cts.Token;
//    //������Token��n��
//    num = await GetInt(_ct);
//    Debug.Log(num);
//    _cts.Cancel();
//}

//private async UniTask<int> GetInt(CancellationToken cancellationToken = default)
//{
//    Debug.Log("int");
//    //���̃��\�b�h�̏�����҂�
//    await UniTask.WaitWhile(() =>
//    {
//        //�L�����Z���̏���
//        cancellationToken.ThrowIfCancellationRequested();
//        //ReturnBoolInMethod����True���Ԃ��Ă��Ă���Ԃ�While�����s����
//        //return ReturnBoolInMethod();
//        Debug.Log("int");
//        return true;
//    });
//    return 1;
//}

//private void Cancel()
//{
//    //Cancel�̃��\�b�h�����s����ƍ����s���Ă���^�X�N���L�����Z���ł���
//    _cts.Cancel();

//    //�L�����Z��Token���Đ������邱�Ƃŕ�����L�����Z�����邱�Ƃ��ł���
//    _cts = new CancellationTokenSource();
//    _ct = _cts.Token;


//}
//async void Start()
//{
//    // CancellationTokenSource�̐���  
//    var cts = new CancellationTokenSource();

//    // CancellationToken�̎擾  
//    CancellationToken token = cts.Token;

//    // Token�𕡐���UniTask�Ɉ����n��  
//    TestUniTask(1, token);
//    TestUniTask(2, token);
//    TestUniTask(3, token);
//    TestUniTask(4, token);


//    await UniTask.Delay(TimeSpan.FromSeconds(1));
//    // 1�b��ɃL�����Z�����������s  
//    cts.Cancel();
//}

//// 5�b��Ƀ��O�o�͂���UniTask  
//async UniTask TestUniTask(int id, CancellationToken token)
//{
//    await UniTask.Delay(TimeSpan.FromSeconds(5));
//    Debug.Log($"UniTask : {id} ����");
//}

//Test1
//GameObject kyaru;
//bool check = true;
//IEnumerator krCoro;

//private void Awake()
//{
//    kyaru = GameObject.Find("Square");
//    //StartCoroutine(StopCoro());
//    krCoro = AccessHelloWorld();
//    StartCoroutine(krCoro);

//}

//private void OnEnable()
//{
//    if (krCoro != null)
//    {
//        StartCoroutine(krCoro);
//    }
//}

//IEnumerator AccessHelloWorld()
//{
//    Debug.Log(kyaru);
//    kyaru.GetComponent<Ru>().HelloWorld();
//    int i = 0;
//    while (true)
//    {
//        Debug.Log(i++);
//        yield return new WaitForSeconds(0.1f); ;
//    }

//    //yield return new WaitForSeconds(5);

//    //Debug.Log(kyaru);
//    //Debug.Log(kyaru.GetComponent<Ru>());
//    //kyaru.GetComponent<Ru>().HelloWorld();
//}

//IEnumerator StopCoro()
//{
//    Debug.Log("�R���[�`�����n�߂���");

//    yield return new WaitForSeconds(2);

//    StopCoroutine(krCoro);
//    Debug.Log("�R���[�`�����~�߂���");

//}
