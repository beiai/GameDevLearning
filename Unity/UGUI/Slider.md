# Slider的常见使用

Slider 继承自 Selectable，所以可以配置一个 TargetGraphic 来显示过渡效果

还可以配置 Fill 填充区域和 Handle 滑动区域

如果 Fill 上的 Image 为 Filled 类型，则 Fill Amount 同步为 Slider 的 normalized

# Slider的本质

提供一个 normalized 的值

在0 - normalized范围内填充一张 Fill 图片

在 normalized 处显示一张 Handle 图片

通过 normalized 算出 Fill 填充区域和 Handle 滑动区域的锚点信息

# Slider点击判定区域

Slider 本身不具有事件检测的能力

但是当 Slider 下面的子节点可被点击检测

但没有处理相关事件时，会交给 Slider 来处理

滑动区域是 Handle 的父节点，如果没有 Handle 则是 Fill 的父节点