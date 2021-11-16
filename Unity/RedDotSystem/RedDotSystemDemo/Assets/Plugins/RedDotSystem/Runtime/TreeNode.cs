using System;
using System.Collections.Generic;

/// <summary>
/// 前缀树节点
/// </summary>
public class TreeNode
{
    /// <summary>
    /// 子节点字典
    /// </summary>
    private Dictionary<string, TreeNode> m_Children;
    
    /// <summary>
    /// 节点值改变回调
    /// </summary>
    private Action<int> m_ValueChangeCallback;

    /// <summary>
    /// 节点名
    /// </summary>
    public string Name
    {
        get;
        private set;
    }
    
    /// <summary>
    /// 节点值
    /// </summary>
    public int Value
    {
        get;
        private set;
    }

    /// <summary>
    /// 父节点
    /// </summary>
    public TreeNode Parent
    {
        get;
        private set;
    }

    /// <summary>
    /// 节点完整路径
    /// </summary>
    private string m_FullPath;
    public string FullPath
    {
        get
        {
            if (string.IsNullOrEmpty(m_FullPath))
            {
                m_FullPath = ReddotManager.Instance.GetNodeFullPath(this);
            }

            return m_FullPath;
        }
    }

    /// <summary>
    /// 子节点
    /// </summary>
    public Dictionary<string, TreeNode>.ValueCollection Children
    {
        get
        {
            return m_Children?.Values;
        }
    }

    /// <summary>
    /// 该节点下所有子节点的数量
    /// </summary>
    public int ChildrenCount
    {
        get
        {
            if (m_Children == null)
            {
                return 0;
            }

            int sum = m_Children.Count;
            foreach (TreeNode node in m_Children.Values)
            {
                sum += node.ChildrenCount;
            }
            return sum;
        }
    }

    public TreeNode(string name)
    {
        Name = name;
        Value = 0;
        m_ValueChangeCallback = null;
    }

    public TreeNode(string name, TreeNode parent) : this(name)
    {
        Parent = parent;
    }

    /// <summary>
    /// 添加节点值变化的监听函数
    /// </summary>
    public void AddListener(Action<int> callback)
    {
        m_ValueChangeCallback += callback;
    }

    /// <summary>
    /// 移除节点值变化的监听函数
    /// </summary>
    public void RemoveListener(Action<int> callback)
    {
        m_ValueChangeCallback -= callback;
    }

    /// <summary>
    /// 移除所有节点值变化的监听函数
    /// </summary>
    public void RemoveAllListener()
    {
        m_ValueChangeCallback = null;
    }

    /// <summary>
    /// 获取子节点，如果不存在则添加
    /// </summary>
    public TreeNode GetOrAddChild(string key)
    {
        TreeNode child = GetChild(key);
        if (child == null)
        {
            child = AddChild(key);
        }
        return child;
    }

    /// <summary>
    /// 获取子节点
    /// </summary>
    public TreeNode GetChild(string key)
    {
        if (m_Children == null)
        {
            return null;
        }

        m_Children.TryGetValue(key, out TreeNode child);
        return child;
    }

    /// <summary>
    /// 添加子节点
    /// </summary>
    public TreeNode AddChild(string key)
    {
        if (m_Children == null)
        {
            m_Children = new Dictionary<string, TreeNode>();
        }
        else if (m_Children.ContainsKey(key))
        {
            throw new Exception("子节点添加失败，不允许重复添加：" + FullPath);
        }

        TreeNode child = new TreeNode(key, this);
        m_Children.Add(key, child);
        ReddotManager.Instance.NodeCountChangeCallback?.Invoke();
        return child;
    }

    /// <summary>
    /// 移除子节点
    /// </summary>
    public bool RemoveChild(string key)
    {
        if (m_Children == null || m_Children.Count == 0)
        {
            return false;
        }

        TreeNode child = GetChild(key);

        if (child != null)
        {
            // 子节点被删除，需要进行一次父节点刷新
            ReddotManager.Instance.MarkDirtyNode(this);
            m_Children.Remove(key);
            ReddotManager.Instance.NodeCountChangeCallback?.Invoke();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 移除所有子节点
    /// </summary>
    public void RemoveAllChild()
    {
        if (m_Children == null || m_Children.Count == 0)
        {
            return;
        }

        m_Children.Clear();
        ReddotManager.Instance.MarkDirtyNode(this);
        ReddotManager.Instance.NodeCountChangeCallback?.Invoke();
    }

    /// <summary>
    /// 改变节点值（使用传入的新值，只能在叶子节点上调用）
    /// </summary>
    public void ChangeValue(int newValue)
    {
        if (m_Children != null && m_Children.Count != 0)
        {
            throw new Exception("不允许直接改变非叶子节点的值：" + FullPath);
        }

        InternalChangeValue(newValue);
    }

    /// <summary>
    /// 改变节点值（根据子节点值计算新值，只对非叶子节点有效）
    /// </summary>
    public void ChangeValue()
    {
        int sum = 0;

        if (m_Children != null && m_Children.Count != 0)
        {
            foreach (KeyValuePair<string, TreeNode> child in m_Children)
            {
                sum += child.Value.Value;
            }
        }

        InternalChangeValue(sum);
    }

    /// <summary>
    /// 改变节点值
    /// </summary>
    private void InternalChangeValue(int newValue)
    {
        if (Value == newValue)
        {
            return;
        }

        Value = newValue;
        m_ValueChangeCallback?.Invoke(newValue);
        ReddotManager.Instance.NodeValueChangeCallback?.Invoke(this, Value);

        //标记父节点为脏节点
        ReddotManager.Instance.MarkDirtyNode(Parent);
    }
    
    public override string ToString()
    {
        return FullPath;
    }
}
