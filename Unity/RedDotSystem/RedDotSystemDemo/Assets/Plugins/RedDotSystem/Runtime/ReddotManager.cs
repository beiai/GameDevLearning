using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 红点管理器
/// </summary>
public class ReddotManager
{
    private static ReddotManager _instance;
    public static ReddotManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ReddotManager();
            }

            return _instance;
        }
    }

    /// <summary>
    /// 所有节点集合
    /// </summary>
    private readonly Dictionary<string, TreeNode> m_AllNodes;

    /// <summary>
    /// 脏节点集合
    /// </summary>
    private readonly HashSet<TreeNode> m_DirtyNodes;

    /// <summary>
    /// 临时脏节点集合
    /// </summary>
    private readonly List<TreeNode> m_TempDirtyNodes;

    /// <summary>
    /// 节点数量改变回调
    /// </summary>
    public Action NodeCountChangeCallback;

    /// <summary>
    /// 节点值改变回调
    /// </summary>
    public Action<TreeNode, int> NodeValueChangeCallback;

    /// <summary>
    /// 路径分隔字符
    /// </summary>
    public char SplitChar { get; private set; }

    /// <summary>
    /// 缓存的StringBuild
    /// </summary>
    public StringBuilder StrBuild { get; private set; }

    /// <summary>
    /// 红点树根节点
    /// </summary>
    public TreeNode Root { get; private set; }

    private ReddotManager()
    {
        SplitChar = '/';
        m_AllNodes = new Dictionary<string, TreeNode>();
        Root = new TreeNode("Root");
        m_DirtyNodes = new HashSet<TreeNode>();
        m_TempDirtyNodes = new List<TreeNode>();
        StrBuild = new StringBuilder();
    }

    /// <summary>
    /// 添加节点值监听
    /// </summary>
    public TreeNode AddListener(string path, Action<int> callback)
    {
        if (callback == null)
        {
            return null;
        }

        TreeNode node = GetTreeNode(path);
        node.AddListener(callback);

        return node;
    }

    /// <summary>
    /// 移除节点值监听
    /// </summary>
    public void RemoveListener(string path, Action<int> callback)
    {
        if (callback == null)
        {
            return;
        }

        TreeNode node = GetTreeNode(path);
        node.RemoveListener(callback);
    }

    /// <summary>
    /// 移除所有节点值监听
    /// </summary>
    public void RemoveAllListener(string path)
    {
        TreeNode node = GetTreeNode(path);
        node.RemoveAllListener();
    }

    /// <summary>
    /// 改变节点值
    /// </summary>
    public void ChangeValue(string path, int newValue)
    {
        TreeNode node = GetTreeNode(path);
        node.ChangeValue(newValue);
    }

    /// <summary>
    /// 获取节点值
    /// </summary>
    public int GetValue(string path)
    {
        TreeNode node = GetTreeNode(path);
        if (node == null)
        {
            return 0;
        }

        return node.Value;
    }

    /// <summary>
    /// 获取指定路径下的节点，自动创建不存在的节点
    /// </summary>
    /// <param name="path">节点的路径</param>
    /// <returns>返回指定路径下的节点</returns>
    public TreeNode GetTreeNode(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new Exception("路径不合法，不能为空");
        }

        if (m_AllNodes.TryGetValue(path, out TreeNode node))
        {
            return node;
        }

        TreeNode currentNode = Root;
        int length = path.Length;

        int startIndex = 0;

        for (int i = 0; i < length; i++)
        {
            if (path[i] == SplitChar)
            {
                if (i == length - 1)
                {
                    throw new Exception("路径不合法，不能以路径分隔符结尾：" + path);
                }

                int endIndex = i - 1;
                if (endIndex < startIndex)
                {
                    throw new Exception("路径不合法，不能存在连续的路径分隔符或以路径分隔符开头：" + path);
                }

                TreeNode child = currentNode.GetOrAddChild(GetSplitString(path, startIndex, endIndex));

                //更新startIndex
                startIndex = i + 1;
                currentNode = child;
            }
        }

        // 最后一个节点，直接用 length - 1 作为 endIndex
        TreeNode target = currentNode.GetOrAddChild(GetSplitString(path, startIndex, length - 1));

        m_AllNodes.Add(path, target);

        return target;
    }

    /// <summary>
    /// 移除节点
    /// </summary>
    public bool RemoveTreeNode(string path)
    {
        if (!m_AllNodes.ContainsKey(path))
        {
            return false;
        }

        TreeNode node = GetTreeNode(path);
        m_AllNodes.Remove(path);
        return node.Parent.RemoveChild(node.Name);
    }

    /// <summary>
    /// 移除所有节点
    /// </summary>
    public void RemoveAllTreeNode()
    {
        Root.RemoveAllChild();
        m_AllNodes.Clear();
    }

    /// <summary>
    /// 管理器轮询
    /// </summary>
    public void Update()
    {
        if (m_DirtyNodes.Count == 0)
        {
            return;
        }

        m_TempDirtyNodes.Clear();
        foreach (TreeNode node in m_DirtyNodes)
        {
            m_TempDirtyNodes.Add(node);
        }

        m_DirtyNodes.Clear();

        //处理所有脏节点
        for (int i = 0; i < m_TempDirtyNodes.Count; i++)
        {
            m_TempDirtyNodes[i].ChangeValue();
        }
    }

    /// <summary>
    /// 标记脏节点
    /// </summary>
    public void MarkDirtyNode(TreeNode node)
    {
        if (node == null || node.Name == Root.Name)
        {
            return;
        }

        m_DirtyNodes.Add(node);
    }

    public string GetSplitString(string path, int startIndex, int endIndex)
    {
        StrBuild.Clear();
        for (int i = startIndex; i <= endIndex; i++)
        {
            StrBuild.Append(path[i]);
        }

        return StrBuild.ToString();
    }

    public string GetNodeFullPath(TreeNode node)
    {
        if (node == Root) return string.Empty;
        var currentNode = node;
        StrBuild.Clear();
        while (true)
        {
            if (currentNode.Parent == null || currentNode.Parent == Root)
            {
                StrBuild.Insert(0, currentNode.Name);
                break;
            }
            StrBuild.Insert(0, currentNode.Name);
            StrBuild.Insert(0, SplitChar);
            currentNode = currentNode.Parent;
        }
        return StrBuild.ToString();
    }
}