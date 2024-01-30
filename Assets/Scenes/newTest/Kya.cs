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
//    //CancellationTokenSourceのインスタンスを生成
//    _cts = new CancellationTokenSource();
//    //_ctsのTokenを生成
//    _ct = _cts.Token;
//    //引数でTokenを渡す
//    num = await GetInt(_ct);
//    Debug.Log(num);
//    _cts.Cancel();
//}

//private async UniTask<int> GetInt(CancellationToken cancellationToken = default)
//{
//    Debug.Log("int");
//    //他のメソッドの処理を待つ
//    await UniTask.WaitWhile(() =>
//    {
//        //キャンセルの処理
//        cancellationToken.ThrowIfCancellationRequested();
//        //ReturnBoolInMethodからTrueが返ってきている間はWhileを実行する
//        //return ReturnBoolInMethod();
//        Debug.Log("int");
//        return true;
//    });
//    return 1;
//}

//private void Cancel()
//{
//    //Cancelのメソッドを実行すると今実行しているタスクをキャンセルできる
//    _cts.Cancel();

//    //キャンセルTokenを再生成することで複数回キャンセルすることができる
//    _cts = new CancellationTokenSource();
//    _ct = _cts.Token;


//}
//async void Start()
//{
//    // CancellationTokenSourceの生成  
//    var cts = new CancellationTokenSource();

//    // CancellationTokenの取得  
//    CancellationToken token = cts.Token;

//    // Tokenを複数のUniTaskに引き渡す  
//    TestUniTask(1, token);
//    TestUniTask(2, token);
//    TestUniTask(3, token);
//    TestUniTask(4, token);


//    await UniTask.Delay(TimeSpan.FromSeconds(1));
//    // 1秒後にキャンセル処理を実行  
//    cts.Cancel();
//}

//// 5秒後にログ出力するUniTask  
//async UniTask TestUniTask(int id, CancellationToken token)
//{
//    await UniTask.Delay(TimeSpan.FromSeconds(5));
//    Debug.Log($"UniTask : {id} 完了");
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
//    Debug.Log("コルーチンを始めたよ");

//    yield return new WaitForSeconds(2);

//    StopCoroutine(krCoro);
//    Debug.Log("コルーチンを止めたよ");

//}
