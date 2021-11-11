# Toggle常见使用

Toggle 最少需要两张 Image，一个用来给 Selectable

一个用来在勾选和非勾选时的图片显示和隐藏

其中 Toggle 和 Image 可在同一个节点

# Toggle的本质

Toggle 组件实际上就是对一个支持点击的对象增加了一个 isOn 的属性

每点击一次就切换 isOn 属性的状态，通过监听 isOn 属性的变化，我们可以做相应的处理

- 只要 isOn 状态发生变化，就处理相应的逻辑，相当于每次点击都执行，类似 Button

- 只当 isOn 为 true 或为 false 时才处理相应的逻辑

通过以上机制完全可以自己实现指定图片的显示和隐藏

# Toggle点击判定区域

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

# 吐槽及优化

ToggleGroup不能获取所有 Toggle

解决方案是继承 ToggleGroup 后可以直接使用 Toggle List

Toggle 只能对单张图的显示隐藏有很大局限性

或者说 Toggle 只适用于 Toggle 这种勾选框

复杂情况还是最好自己手撸魔改一个

