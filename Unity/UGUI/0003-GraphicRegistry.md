# GraphicRegistry

所有继承自 Graphic 的类都会都会注册到 GraphicRegistry

记录 Graphic 位于哪个 Canvas 之中，同时记录该 Graphic 是否可以被射线检测 

## Graphic.OnEnable()

激活时注册到 GraphicRegistry

```c#
// 获取距离该 Graphic 组件最近的 Canvas
CacheCanvas();
// 注册到 GraphicRegistry
GraphicRegistry.RegisterGraphicForCanvas(canvas, this);
```

## Graphic.OnDisable()

禁用时从 GraphicRegistry 中取消注册

```c#
GraphicRegistry.UnregisterGraphicForCanvas(canvas, this);
```

## Graphic.OnBeforeTransformParentChanged()

在 OnTransformParentChanged 之前从 GraphicRegistry 中取消注册

```c#
GraphicRegistry.UnregisterGraphicForCanvas(canvas, this);
```

## Graphic.OnTransformParentChanged()

父节点关系改变时注册到 GraphicRegistry

```c#
// 获取距离该 Graphic 组件最近的 Canvas
CacheCanvas();
// 注册到 GraphicRegistry
GraphicRegistry.RegisterGraphicForCanvas(canvas, this);
```

## Graphic.OnCanvasHierarchyChanged()

当层级面板中的 Canvas 改变时注册到 GraphicRegistry

```c#
// 获取距离该 Graphic 组件最近的 Canvas
CacheCanvas();
// 当距离该 Graphic 组件最近的 Canvas 和之前不同时
// 从 GraphicRegistry 移除旧的注册
GraphicRegistry.UnregisterGraphicForCanvas(currentCanvas, this);
// 注册新的到 GraphicRegistry
GraphicRegistry.RegisterGraphicForCanvas(canvas, this);
```

# GetGraphicsForCanvas()

根据 Canvas 获取该 Canvas 下所有继承自 Graphic 的组件

# GetRaycastableGraphicsForCanvas()

根据 Canvas 获取该 Canvas 下所有继承自 Graphic 且开启了 Raycastable 的组件