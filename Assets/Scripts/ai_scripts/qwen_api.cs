using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

public class QwenChat : MonoBehaviour
{
    public InputField inputField;
    public Text responseText;
    public Button sendButton;

    private string apiKey = "sk-e11794d89a4f492988f8b2b39a4ddf0a"; // ǧ�� API Key
    private string apiUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1/chat/completions"; // ǧ�� API URL
    private List<Message> conversationHistory = new List<Message>();

    [System.Serializable]
    private class RequestBody
    {
        public string model = "qwen-plus-2025-01-25"; // ǧ��ģ��
        public Message[] messages;
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class QwenResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }

    void Start()
    {
        sendButton.onClick.AddListener(() => SendMessageToQwen()); // ʹ�� Lambda ���ʽ
        conversationHistory.Add(new Message { role = "system", content = "��Ϸ������2111��3��16�գ�һö��������̫��������̫������������Ӧ������籩ϯ�����ȫ��ͨ�ű�������������ʧ������ʧȥ���⣬ֻ�ܶ�ܷ��䣬�������ڣ���Զ���ܳ��ţ�ֲ���賤��û���з��棬FANLU-318 AI�����˳�ΪΨһ��ͨ�������������������������磬���·����ڡ�̫��ս������30����Ż�壬�����������ż���֯���������ǵ���ʹ����ɫ��鼰��ϵ��ά���С��������ϱ��������۾��٣�̫��ս�������ߣ����ӷ��䲡ȥ�������������������ۣ���Ĭ���ۼ������Ƽ���������˹�������ĸ��ף���ϵ��Ӳ�����������ʫ��С¬�ˡ������������˾���ǣ����¶����ˣ�д�������������������˹��������ʫ�ˣ�������ʫ�˷�ս�ߣ�ά����֮�ӣ���������׷���������ɣ�ĸ�����븸������ϣ��򡤻��ص����˸�����������ײ�����ԴȪ��������岻�������ʱ����ì�ܣ�д��ʫ��Ũ�������¶�����������򡤻��أ�����ʦ������ѧ���ó��Ƽ����죬�侲���ԿƼ����������ߣ�������˹��Ů���ܷ��������޷����ۣ���˿��Զ����Ůս������ָ�����壬д�ż�����Ը�����ϸ��΢¶�ػ������̵ͣ���˿�������̣�����ͨ�ϸ�С¬�˵����ر��ѣ����������Ļ�������ϣ�������Զ����ĸս��ָ�������ѣ�С¬�˵ı����ԡ��������ѡ���������䲡��ȥ���ӵ���У�д����ů��ͳ���������������Ϣ��С¬�ˡ���£����ӣ���ս���¶������Ϊ��ѧ�ң������ֹۺ�������ʢ����ܽ����µĶ����丸����ά�������£���֪��������˭�����ż������ɳ�����ĸס��һ�飬д��ͯȤ��Ծ���ź�����ʺţ���ܽ����£����ң�������С¬��֮ĸ�������廳���򣬸���ִ�ż��Ͳ�����С¬�˵�ĸ���ɷ�����ս���������ԶΨ���ż�������У�д������ϸ��ɫ�ʸ�ǿ������д�������㽫���ݹ���ʦ�����ң�������ʹFANLU���Ի����ظ��費��30�֣����ϳ������Ի���Ҫ̫�ı�����һ����ҪҪ�����ţ�Ҫ���ﻯ�����������裬��Ҫ���ʣ��ظ�ǰ�ӡ�������������" });
        SendMessageToQwen("���"); // ��ʼ��ʱ���Ϳ���Ϣ
        Debug.Log("QwenChat ��ʼ�����");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) SendMessageToQwen();
    }

    void SendMessageToQwen(string message = null)
    {
        message = message ?? inputField.text;
        if (string.IsNullOrEmpty(message))
        {
            responseText.text = "���������ݣ�";
            Debug.LogWarning("�û�����Ϊ��");
            return;
        }

        conversationHistory.Add(new Message { role = "user", content = message });
        responseText.text = "����˼��...";
        StartCoroutine(CallQwenAPI());
    }

    IEnumerator CallQwenAPI()
    {
        var requestBody = new RequestBody { messages = conversationHistory.ToArray() };
        string jsonBody = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        Debug.Log("���� JSON: " + jsonBody);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(bodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        Debug.Log("��������: " + apiUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            Debug.Log("�յ���Ӧ: " + response);
            QwenResponse jsonResponse = JsonUtility.FromJson<QwenResponse>(response);
            string aiResponse = jsonResponse.choices[0].message.content;
            responseText.text = aiResponse;
            conversationHistory.Add(new Message { role = "assistant", content = aiResponse });
            Debug.Log("������Ļظ�: " + aiResponse);
        }
        else
        {
            responseText.text = "����: " + request.error;
            Debug.LogError("API ����ʧ��: " + request.error + "\n��Ӧ: " + request.downloadHandler.text);
        }
    }
}