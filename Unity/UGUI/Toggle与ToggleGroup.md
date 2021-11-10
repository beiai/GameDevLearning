# Toggle

Toggle 最少需要两张 Image，一个用来给 Selectable

一个用来在勾选和非勾选时的图片显示和隐藏

其中 Toggle 和 Image 可在同一个节点

# Toggle 点击判定区域

Toggle 本身不具有事件检测的能力

但是当 Toggle 下面的子节点可被点击检测

但没有处理 IPointerClickHandler，ISubmitHandler 事件时

会交给父节点上的 IPointerClickHandler，ISubmitHandler 来处理

也就是说，Toggle 节点下，要至少存在一个可被点击检测的对象

该组件才能正常执行，判定区域就是子节点的判定区域

# SetIsOnWithoutNotify

设置 Toggle Is On 的值，但不会触发 OnValueChanged 回调

# ToggleGroup

在 Toggle 上设置 ToggleGroup 时，都会在 ToggleGroup 中注册该 Toggle

```c#
NotifyToggleOn
AnyTogglesOn
ActiveToggles
GetFirstActiveToggle
```

GetFirstActiveToggle 函数可以获取当前激活的 Toggle，感觉就这个比较实用

# ToggleGroup事件响应机制

切换 ToggleGroup 中的 Toggle 时

先触发 true -> false 的 OnValueChanged(false)

再触发 false -> true 的 OnValueChanged(true)

