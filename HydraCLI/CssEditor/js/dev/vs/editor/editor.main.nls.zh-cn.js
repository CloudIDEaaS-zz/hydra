/*!-----------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Version: 0.32.1(8ad5e3bceab16a4d0856c43a374b511dffb1e795)
 * Released under the MIT license
 * https://github.com/microsoft/vscode/blob/main/LICENSE.txt
 *-----------------------------------------------------------*/

define("vs/editor/editor.main.nls.zh-cn", {
	"vs/base/browser/ui/actionbar/actionViewItems": [
		"{0} ({1})",
	],
	"vs/base/browser/ui/findinput/findInput": [
		"输入",
	],
	"vs/base/browser/ui/findinput/findInputCheckboxes": [
		"区分大小写",
		"全字匹配",
		"使用正则表达式",
	],
	"vs/base/browser/ui/findinput/replaceInput": [
		"输入",
		"保留大小写",
	],
	"vs/base/browser/ui/iconLabel/iconLabelHover": [
		"正在加载…",
	],
	"vs/base/browser/ui/inputbox/inputBox": [
		"错误: {0}",
		"警告: {0}",
		"信息: {0}",
		"对于历史记录",
	],
	"vs/base/browser/ui/keybindingLabel/keybindingLabel": [
		"未绑定",
	],
	"vs/base/browser/ui/tree/abstractTree": [
		"清除",
		"禁用输入时筛选",
		"启用输入时筛选",
		"未找到元素",
		"已匹配 {0} 个元素(共 {1} 个)",
	],
	"vs/base/common/actions": [
		"(空)",
	],
	"vs/base/common/errorMessage": [
		"{0}: {1}",
		"发生了系统错误 ({0})",
		"出现未知错误。有关详细信息，请参阅日志。",
		"出现未知错误。有关详细信息，请参阅日志。",
		"{0} 个(共 {1} 个错误)",
		"出现未知错误。有关详细信息，请参阅日志。",
	],
	"vs/base/common/keybindingLabels": [
		"Ctrl",
		"Shift",
		"Alt",
		"Windows",
		"Ctrl",
		"Shift",
		"Alt",
		"超键",
		"Control",
		"Shift",
		"选项",
		"Command",
		"Control",
		"Shift",
		"Alt",
		"Windows",
		"Control",
		"Shift",
		"Alt",
		"超键",
	],
	"vs/base/parts/quickinput/browser/quickInput": [
		"上一步",
		"按 \"Enter\" 以确认或按 \"Esc\" 以取消",
		"{0}/{1}",
		"在此输入可缩小结果范围。",
		"{0} 个结果",
		"已选 {0} 项",
		"确定",
		"自定义",
		"后退 ({0})",
		"上一步",
	],
	"vs/base/parts/quickinput/browser/quickInputList": [
		"快速输入",
	],
	"vs/editor/browser/controller/coreCommands": [
		"即使转到较长的行，也一直到末尾",
		"即使转到较长的行，也一直到末尾",
		"已删除辅助游标",
	],
	"vs/editor/browser/controller/textAreaHandler": [
		"编辑器",
		"现在无法访问编辑器。按 {0} 获取选项。",
	],
	"vs/editor/browser/editorExtensions": [
		"撤消(&&U)",
		"撤消",
		"恢复(&&R)",
		"恢复",
		"全选(&&S)",
		"选择全部",
	],
	"vs/editor/browser/widget/codeEditorWidget": [
		"光标数量被限制为 {0}。",
	],
	"vs/editor/browser/widget/diffEditorWidget": [
		"差异编辑器中插入项的线条修饰。",
		"差异编辑器中删除项的线条修饰。",
		"文件过大，无法比较。",
	],
	"vs/editor/browser/widget/diffReview": [
		"差异评审中的“插入”图标。",
		"差异评审中的“删除”图标。",
		"差异评审中的“关闭”图标。",
		"关闭",
		"未更改行",
		"更改了 1 行",
		"更改了 {0} 行",
		"差异 {0}/ {1}: 原始行 {2}，{3}，修改后的行 {4}，{5}",
		"空白",
		"{0} 未更改的行 {1}",
		"{0}原始行{1}修改的行{2}",
		"+ {0}修改的行{1}",
		"- {0}原始行{1}",
		"转至下一个差异",
		"转至上一个差异",
	],
	"vs/editor/browser/widget/inlineDiffMargin": [
		"复制已删除的行",
		"复制已删除的行",
		"复制更改的行",
		"复制更改的行",
		"复制已删除的行({0})",
		"复制更改的行({0})",
		"还原此更改",
		"复制已删除的行({0})",
		"复制更改的行({0})",
	],
	"vs/editor/common/config/editorConfigurationSchema": [
		"编辑器",
		"一个制表符等于的空格数。在 `#editor.detectIndentation#` 启用时，根据文件内容，该设置可能会被覆盖。",
		"按 `Tab` 键时插入空格。该设置在 `#editor.detectIndentation#` 启用时根据文件内容可能会被覆盖。",
		"控制是否在打开文件时，基于文件内容自动检测 `#editor.tabSize#` 和 `#editor.insertSpaces#`。",
		"删除自动插入的尾随空白符号。",
		"对大型文件进行特殊处理，禁用某些内存密集型功能。",
		"控制是否根据文档中的文字计算自动完成列表。",
		"仅建议活动文档中的字词。",
		"建议使用同一语言的所有打开的文档中的字词。",
		"建议所有打开的文档中的字词。",
		"控制通过哪些文档计算基于字词的补全。",
		"对所有颜色主题启用语义突出显示。",
		"对所有颜色主题禁用语义突出显示。",
		"语义突出显示是由当前颜色主题的 \"semanticHighlighting\" 设置配置的。",
		"控制是否为支持它的语言显示语义突出显示。",
		"在速览编辑器中，即使双击其中的内容或者按 `Esc` 键，也保持其打开状态。",
		"由于性能原因，超过这个长度的行将不会被标记",
		"定义增加和减少缩进的括号。",
		"左方括号字符或字符串序列。",
		"右方括号字符或字符串序列。",
		"如果启用方括号对着色，则按照其嵌套级别定义已着色的方括号对。",
		"左方括号字符或字符串序列。",
		"右方括号字符或字符串序列。",
		"超时(以毫秒为单位)，之后将取消差异计算。使用0表示没有超时。",
		"要为其计算差异的最大文件大小(MB)。使用 0 表示无限制。",
		"控制差异编辑器的显示方式是并排还是内联。",
		"启用后，差异编辑器将忽略前导空格或尾随空格中的更改。",
		"控制差异编辑器是否为添加/删除的更改显示 +/- 指示符号。",
		"控制是否在编辑器中显示 CodeLens。",
		"永不换行。",
		"将在视区宽度处换行。",
		"将根据 `#editor.wordWrap#` 设置换行。",
	],
	"vs/editor/common/config/editorOptions": [
		"编辑器将使用平台 API 以检测是否附加了屏幕阅读器。",
		"编辑器将针对与屏幕阅读器搭配使用进行永久优化。将禁用自动换行。",
		"编辑器将不再对屏幕阅读器的使用进行优化。",
		"控制编辑器是否应在对屏幕阅读器进行了优化的模式下运行。设置为“开”将禁用自动换行。",
		"控制在注释时是否插入空格字符。",
		"控制在对行注释执行切换、添加或删除操作时，是否应忽略空行。",
		"控制在没有选择内容时进行复制是否复制当前行。",
		"控制在键入时光标是否应跳转以查找匹配项。",
		"切勿为编辑器选择中的搜索字符串设定种子。",
		"始终为编辑器选择中的搜索字符串设定种子，包括光标位置的字词。",
		"仅为编辑器选择中的搜索字符串设定种子。",
		"控制是否将编辑器选中内容作为搜索词填入到查找小组件中。",
		"从不自动打开“在选定内容中查找”(默认)。",
		"始终自动打开“在选定内容中查找”。",
		"选择多行内容时，自动打开“在选定内容中查找”。",
		"控制自动打开“在选定内容中查找”的条件。",
		"控制“查找”小组件是否读取或修改 macOS 的共享查找剪贴板。",
		"控制 \"查找小部件\" 是否应在编辑器顶部添加额外的行。如果为 true, 则可以在 \"查找小工具\" 可见时滚动到第一行之外。",
		"控制在找不到其他匹配项时，是否自动从开头(或结尾)重新开始搜索。",
		"启用/禁用字体连字(\"calt\" 和 \"liga\" 字体特性)。将此更改为字符串，可对 \"font-feature-settings\" CSS 属性进行精细控制。",
		"显式 \"font-feature-settings\" CSS 属性。如果只需打开/关闭连字，可以改为传递布尔值。",
		"配置字体连字或字体特性。可以是用于启用/禁用连字的布尔值，或用于设置 CSS \"font-feature-settings\" 属性值的字符串。",
		"控制字体大小(像素)。",
		"仅允许使用关键字“正常”和“加粗”，或使用介于 1 至 1000 之间的数字。",
		"控制字体粗细。接受关键字“正常”和“加粗”，或者接受介于 1 至 1000 之间的数字。",
		"显示结果的预览视图 (默认值)",
		"转到主结果并显示预览视图",
		"转到主结果，并对其他人启用防偷窥导航",
		"此设置已弃用，请改用单独的设置，如\"editor.editor.gotoLocation.multipleDefinitions\"或\"editor.editor.gotoLocation.multipleImplementations\"。",
		"控制存在多个目标位置时\"转到定义\"命令的行为。",
		"控制存在多个目标位置时\"转到类型定义\"命令的行为。",
		"控制存在多个目标位置时\"转到声明\"命令的行为。",
		"控制存在多个目标位置时\"转到实现\"命令的行为。",
		"控制存在多个目标位置时\"转到引用\"命令的行为。",
		"当\"转到定义\"的结果为当前位置时将要执行的替代命令的 ID。",
		"当\"转到类型定义\"的结果是当前位置时正在执行的备用命令 ID。",
		"当\"转到声明\"的结果为当前位置时将要执行的替代命令的 ID。",
		"当\"转到实现\"的结果为当前位置时将要执行的替代命令的 ID。",
		"当\"转到引用\"的结果是当前位置时正在执行的替代命令 ID。",
		"控制是否显示悬停提示。",
		"控制显示悬停提示前的等待时间 (毫秒)。",
		"控制当鼠标移动到悬停提示上时，其是否保持可见。",
		"如果有空间，首选在线条上方显示悬停。",
		"在编辑器中启用代码操作小灯泡提示。",
		"在编辑器中启用内联提示。",
		"控制编辑器中内嵌提示的字号。当配置的值小于 `5` 或大于编辑器字号时，默认使用 90% 的 `#editor.fontSize#`。",
		"在编辑器中控制内嵌提示的字体系列。设置为空时，使用 `#editor.fontFamily#`。",
		"控制行高。\r\n - 使用 0 根据字号自动计算行高。\r\n - 介于 0 和 8 之间的值将用作字号的乘数。\r\n - 大于或等于 8 的值将用作有效值。",
		"控制是否显示缩略图。",
		"迷你地图的大小与编辑器内容相同(并且可能滚动)。",
		"迷你地图将根据需要拉伸或缩小以填充编辑器的高度(不滚动)。",
		"迷你地图将根据需要缩小，永远不会大于编辑器(不滚动)。",
		"控制迷你地图的大小。",
		"控制在哪一侧显示缩略图。",
		"控制何时显示迷你地图滑块。",
		"在迷你地图中绘制的内容比例: 1、2 或 3。",
		"渲染每行的实际字符，而不是色块。",
		"限制缩略图的宽度，控制其最多显示的列数。",
		"控制编辑器的顶边和第一行之间的间距量。",
		"控制编辑器的底边和最后一行之间的间距量。",
		"在输入时显示含有参数文档和类型信息的小面板。",
		"控制参数提示菜单在到达列表末尾时进行循环还是关闭。",
		"在字符串内启用快速建议。",
		"在注释内启用快速建议。",
		"在字符串和注释外启用快速建议。",
		"控制是否在键入时自动显示建议。",
		"不显示行号。",
		"将行号显示为绝对行数。",
		"将行号显示为与光标相隔的行数。",
		"每 10 行显示一次行号。",
		"控制行号的显示。",
		"此编辑器标尺将渲染的等宽字符数。",
		"此编辑器标尺的颜色。",
		"在一定数量的等宽字符后显示垂直标尺。输入多个值，显示多个标尺。若数组为空，则不绘制标尺。",
		"垂直滚动条仅在必要时可见。",
		"垂直滚动条将始终可见。",
		"垂直滚动条将始终隐藏。",
		"控制垂直滚动条的可见性。",
		"水平滚动条仅在必要时可见。",
		"水平滚动条将始终可见。",
		"水平滚动条将始终隐藏。",
		"控制水平滚动条的可见性。",
		"垂直滚动条的宽度。",
		"水平滚动条的高度。",
		"控制单击按页滚动还是跳转到单击位置。",
		"控制是否突出显示所有非基本 ASCII 字符。只有介于 U+0020 到 U+007E 之间的字符、制表符、换行符和回车符才被视为基本 ASCII。",
		"控制是否突出显示仅保留空格或完全没有宽度的字符。",
		"控制是否突出显示可能与基本 ASCII 字符混淆的字符，但当前用户区域设置中常见的字符除外。",
		"控制注释中的字符是否也应进行 Unicode 突出显示。",
		"控制字符串中的字符是否也应进行 unicode 突出显示。",
		"定义未突出显示的允许字符。",
		"未突出显示在允许区域设置中常见的 Unicode 字符。",
		"控制是否在编辑器中自动显示内联建议。",
		"控制是否启用括号对着色。使用 “workbench.colorCustomizations” 替代括号突出显示颜色。",
		"启用括号对参考线。",
		"仅为活动括号对启用括号对参考线。",
		"禁用括号对参考线。",
		"控制是否启用括号对指南。",
		"启用水平参考线作为垂直括号对参考线的添加项。",
		"仅为活动括号对启用水平参考线。",
		"禁用水平括号对参考线。",
		"控制是否启用水平括号对指南。",
		"控制是否启用括号对指南。",
		"控制编辑器是否显示缩进参考线。",
		"控制是否突出显示编辑器中活动的缩进参考线。",
		"插入建议而不覆盖光标右侧的文本。",
		"插入建议并覆盖光标右侧的文本。",
		"控制接受补全时是否覆盖单词。请注意，这取决于扩展选择使用此功能。",
		"控制对建议的筛选和排序是否考虑小的拼写错误。",
		"控制排序时是否首选光标附近的字词。",
		"控制是否在多个工作区和窗口间共享记忆的建议选项(需要 `#editor.suggestSelection#`)。",
		"控制活动代码段是否阻止快速建议。",
		"控制是否在建议中显示或隐藏图标。",
		"控制建议小部件底部的状态栏的可见性。",
		"控制是否在编辑器中预览建议结果。",
		"控制建议详细信息是随标签一起显示还是仅显示在详细信息小组件中",
		"此设置已弃用。现在可以调整建议小组件的大小。",
		"此设置已弃用，请改用单独的设置，如\"editor.suggest.showKeywords\"或\"editor.suggest.showSnippets\"。",
		"启用后，IntelliSense 将显示“方法”建议。",
		"启用后，IntelliSense 将显示“函数”建议。",
		"启用后，IntelliSense 将显示“构造函数”建议。",
		"启用后，IntelliSense 将显示“已启用”建议。",
		"启用后，IntelliSense 将显示“字段”建议。",
		"启用后，IntelliSense 将显示“变量”建议。",
		"启用后，IntelliSense 将显示“类”建议。",
		"启用后，IntelliSense 将显示“结构”建议。",
		"启用后，IntelliSense 将显示“接口”建议。",
		"启用后，IntelliSense 将显示“模块”建议。",
		"启用后，IntelliSense 将显示“属性”建议。",
		"启用后，IntelliSense 将显示“事件”建议。",
		"启用后，IntelliSense 将显示“操作符”建议。",
		"启用后，IntelliSense 将显示“单位”建议。",
		"启用后，IntelliSense 将显示“值”建议。",
		"启用后，IntelliSense 将显示“常量”建议。",
		"启用后，IntelliSense 将显示“枚举”建议。",
		"启用后，IntelliSense 将显示 \"enumMember\" 建议。",
		"启用后，IntelliSense 将显示“关键字”建议。",
		"启用后，IntelliSense 将显示“文本”建议。",
		"启用后，IntelliSense 将显示“颜色”建议。",
		"启用后，IntelliSense 将显示“文件”建议。",
		"启用后，IntelliSense 将显示“参考”建议。",
		"启用后，IntelliSense 将显示“自定义颜色”建议。",
		"启用后，IntelliSense 将显示“文件夹”建议。",
		"启用后，IntelliSense 将显示 \"typeParameter\" 建议。",
		"启用后，IntelliSense 将显示“片段”建议。",
		"启用后，IntelliSense 将显示\"用户\"建议。",
		"启用后，IntelliSense 将显示\"问题\"建议。",
		"是否应始终选择前导和尾随空格。",
		"控制是否应在遇到提交字符时接受建议。例如，在 JavaScript 中，半角分号 (`;`) 可以为提交字符，能够在接受建议的同时键入该字符。",
		"仅当建议包含文本改动时才可使用 `Enter` 键进行接受。",
		"控制除了 `Tab` 键以外， `Enter` 键是否同样可以接受建议。这能减少“插入新行”和“接受建议”命令之间的歧义。",
		"控制编辑器中可由屏幕阅读器一次读出的行数。我们检测到屏幕阅读器时，会自动将默认值设置为 500。警告: 如果行数大于默认值，可能会影响性能。",
		"编辑器内容",
		"使用语言配置确定何时自动闭合括号。",
		"仅当光标位于空白字符左侧时，才自动闭合括号。",
		"控制编辑器是否在左括号后自动插入右括号。",
		"仅在自动插入时才删除相邻的右引号或右括号。",
		"控制在删除时编辑器是否应删除相邻的右引号或右方括号。",
		"仅在自动插入时才改写右引号或右括号。",
		"控制编辑器是否应改写右引号或右括号。",
		"使用语言配置确定何时自动闭合引号。",
		"仅当光标位于空白字符左侧时，才自动闭合引号。",
		"控制编辑器是否在左引号后自动插入右引号。",
		"编辑器不会自动插入缩进。",
		"编辑器将保留当前行的缩进。",
		"编辑器将保留当前行的缩进并遵循语言定义的括号。",
		"编辑器将保留当前行的缩进、使用语言定义的括号并调用语言定义的特定 onEnterRules。",
		"编辑器将保留当前行的缩进，使用语言定义的括号，调用由语言定义的特殊输入规则，并遵循由语言定义的缩进规则。",
		"控制编辑器是否应在用户键入、粘贴、移动或缩进行时自动调整缩进。",
		"使用语言配置确定何时自动包住所选内容。",
		"使用引号而非括号来包住所选内容。",
		"使用括号而非引号来包住所选内容。",
		"控制在键入引号或方括号时，编辑器是否应自动将所选内容括起来。",
		"在使用空格进行缩进时模拟制表符的选择行为。所选内容将始终使用制表符停止位。",
		"控制是否在编辑器中显示 CodeLens。",
		"控制 CodeLens 的字体系列。",
		"控制 CodeLens 的字号(以像素为单位)。设置为 `0` 时，将使用 90% 的 `#editor.fontSize#`。",
		"控制编辑器是否显示内联颜色修饰器和颜色选取器。",
		"启用使用鼠标和键进行列选择。",
		"控制在复制时是否同时复制语法高亮。",
		"控制光标的动画样式。",
		"控制是否启用平滑插入动画。",
		"控制光标样式。",
		"控制光标周围可见的前置行和尾随行的最小数目。在其他一些编辑器中称为 \"scrollOff\" 或 \"scrollOffset\"。",
		"仅当通过键盘或 API 触发时，才会强制执行\"光标环绕行\"。",
		"始终强制执行 \"cursorSurroundingLines\"",
		"控制何时应强制执行\"光标环绕行\"。",
		"当 `#editor.cursorStyle#` 设置为 `line` 时，控制光标的宽度。",
		"控制在编辑器中是否允许通过拖放来移动选中内容。",
		"按下\"Alt\"时滚动速度倍增。",
		"控制编辑器是否启用了代码折叠。",
		"使用特定于语言的折叠策略(如果可用)，否则使用基于缩进的策略。",
		"使用基于缩进的折叠策略。",
		"控制计算折叠范围的策略。",
		"控制编辑器是否应突出显示折叠范围。",
		"控制编辑器是否自动折叠导入范围。",
		"可折叠区域的最大数量。如果当前源具有大量可折叠区域，那么增加此值可能会导致编辑器的响应速度变慢。",
		"控制单击已折叠的行后面的空内容是否会展开该行。",
		"控制字体系列。",
		"控制编辑器是否自动格式化粘贴的内容。格式化程序必须可用，并且能针对文档中的某一范围进行格式化。",
		"控制编辑器在键入一行后是否自动格式化该行。",
		"控制编辑器是否应呈现垂直字形边距。字形边距最常用于调试。",
		"控制是否在概览标尺中隐藏光标。",
		"控制字母间距(像素)。",
		"控制编辑器是否已启用链接编辑。相关符号(如 HTML 标记)在编辑时进行更新，具体由语言而定。",
		"控制是否在编辑器中检测链接并使其可被点击。",
		"突出显示匹配的括号。",
		"对鼠标滚轮滚动事件的 `deltaX` 和 `deltaY` 乘上的系数。",
		"按住 `Ctrl` 键并滚动鼠标滚轮时对编辑器字体大小进行缩放。",
		"当多个光标重叠时进行合并。",
		"映射为 `Ctrl` (Windows 和 Linux) 或 `Command` (macOS)。",
		"映射为 `Alt` (Windows 和 Linux) 或 `Option` (macOS)。",
		"在通过鼠标添加多个光标时使用的修改键。“转到定义”和“打开链接”功能所需的鼠标动作将会相应调整，不与多光标修改键冲突。[阅读详细信息](https://code.visualstudio.com/docs/editor/codebasics#_multicursor-modifier)。",
		"每个光标粘贴一行文本。",
		"每个光标粘贴全文。",
		"控制粘贴时粘贴文本的行计数与光标计数相匹配。",
		"控制编辑器是否突出显示语义符号的匹配项。",
		"控制是否在概览标尺周围绘制边框。",
		"打开速览时聚焦树",
		"打开预览时将焦点放在编辑器上",
		"控制是将焦点放在内联编辑器上还是放在预览小部件中的树上。",
		"控制\"转到定义\"鼠标手势是否始终打开预览小部件。",
		"控制显示快速建议前的等待时间 (毫秒)。",
		"控制是否在编辑器中输入时自动重命名。",
		"已弃用，请改用 \"editor.linkedEditing\"。",
		"控制编辑器是否显示控制字符。",
		"当文件以换行符结束时, 呈现最后一行的行号。",
		"同时突出显示导航线和当前行。",
		"控制编辑器的当前行进行高亮显示的方式。",
		"控制编辑器是否仅在焦点在编辑器时突出显示当前行。",
		"呈现空格字符(字词之间的单个空格除外)。",
		"仅在选定文本上呈现空白字符。",
		"仅呈现尾随空格字符。",
		"控制编辑器在空白字符上显示符号的方式。",
		"控制选区是否有圆角。",
		"控制编辑器水平滚动时可以超过范围的字符数。",
		"控制编辑器是否可以滚动到最后一行之后。",
		"同时垂直和水平滚动时，仅沿主轴滚动。在触控板上垂直滚动时，可防止水平漂移。",
		"控制是否支持 Linux 主剪贴板。",
		"控制编辑器是否应突出显示与所选内容类似的匹配项。",
		"始终显示折叠控件。",
		"仅在鼠标位于装订线上方时显示折叠控件。",
		"控制何时显示行号槽上的折叠控件。",
		"控制是否淡化未使用的代码。",
		"控制加删除线被弃用的变量。",
		"在其他建议上方显示代码片段建议。",
		"在其他建议下方显示代码片段建议。",
		"在其他建议中穿插显示代码片段建议。",
		"不显示代码片段建议。",
		"控制代码片段是否与其他建议一起显示及其排列的位置。",
		"控制编辑器是否使用动画滚动。",
		"建议小部件的字号。如果设置为 `0`，则使用 `#editor.fontSize#` 的值。",
		"建议小部件的行高。如果设置为 `0`，则使用 `#editor.lineHeight#` 的值。最小值为 8。",
		"控制在键入触发字符后是否自动显示建议。",
		"始终选择第一个建议。",
		"选择最近的建议，除非进一步键入选择其他项。例如 `console. -> console.log`，因为最近补全过 `log`。",
		"根据之前补全过的建议的前缀来进行选择。例如，`co -> console`、`con -> const`。",
		"控制在建议列表中如何预先选择建议。",
		"在按下 Tab 键时进行 Tab 补全，将插入最佳匹配建议。",
		"禁用 Tab 补全。",
		"在前缀匹配时进行 Tab 补全。在 \"quickSuggestions\" 未启用时体验最好。",
		"启用 Tab 补全。",
		"自动删除异常的行终止符。",
		"忽略异常的行终止符。",
		"提示删除异常的行终止符。",
		"删除可能导致问题的异常行终止符。",
		"根据制表位插入和删除空格。",
		"执行单词相关的导航或操作时作为单词分隔符的字符。",
		"永不换行。",
		"将在视区宽度处换行。",
		"在 `#editor.wordWrapColumn#` 处折行。",
		"在视区宽度和 `#editor.wordWrapColumn#` 中的较小值处折行。",
		"控制折行的方式。",
		"在 `#editor.wordWrap#` 为 `wordWrapColumn` 或 `bounded` 时，控制编辑器的折行列。",
		"没有缩进。折行从第 1 列开始。",
		"折行的缩进量与其父级相同。",
		"折行的缩进量比其父级多 1。",
		"折行的缩进量比其父级多 2。",
		"控制折行的缩进。",
		"假定所有字符的宽度相同。这是一种快速算法，适用于等宽字体和某些字形宽度相等的文字(如拉丁字符)。",
		"将包装点计算委托给浏览器。这是一个缓慢算法，可能会导致大型文件被冻结，但它在所有情况下都正常工作。",
		"控制计算包裹点的算法。",
	],
	"vs/editor/common/core/editorColorRegistry": [
		"光标所在行高亮内容的背景颜色。",
		"光标所在行四周边框的背景颜色。",
		"背景颜色的高亮范围，喜欢通过快速打开和查找功能。颜色不能不透明，以免隐藏底层装饰。",
		"高亮区域边框的背景颜色。",
		"高亮显示符号的背景颜色，例如转到定义或转到下一个/上一个符号。颜色不能是不透明的，以免隐藏底层装饰。",
		"高亮显示符号周围的边框的背景颜色。",
		"编辑器光标颜色。",
		"编辑器光标的背景色。可以自定义块型光标覆盖字符的颜色。",
		"编辑器中空白字符的颜色。",
		"编辑器缩进参考线的颜色。",
		"编辑器活动缩进参考线的颜色。",
		"编辑器行号的颜色。",
		"编辑器活动行号的颜色",
		"\"Id\" 已被弃用，请改用 \"editorLineNumber.activeForeground\"。",
		"编辑器活动行号的颜色",
		"编辑器标尺的颜色。",
		"编辑器 CodeLens 的前景色",
		"匹配括号的背景色",
		"匹配括号外框的颜色",
		"概览标尺边框的颜色。",
		"编辑器概述标尺的背景色。仅当缩略图已启用且置于编辑器右侧时才使用。",
		"编辑器导航线的背景色。导航线包括边缘符号和行号。",
		"编辑器中不必要(未使用)的源代码的边框颜色。",
		"非必须(未使用)代码的在编辑器中显示的不透明度。例如，\"#000000c0\" 将以 75% 的不透明度显示代码。对于高对比度主题，请使用 ”editorUnnecessaryCode.border“ 主题来为非必须代码添加下划线，以避免颜色淡化。",
		"编辑器中虚影文本的边框颜色。",
		"编辑器中虚影文本的前景色。",
		"编辑器中虚影文本的背景色。",
		"用于突出显示范围的概述标尺标记颜色。颜色必须透明，以免隐藏下面的修饰效果。",
		"概览标尺中错误标记的颜色。",
		"概览标尺中警告标记的颜色。",
		"概览标尺中信息标记的颜色。",
		"括号的前景色(1)。需要启用括号对着色。",
		"括号的前景色(2)。需要启用括号对着色。",
		"括号的前景色(3)。需要启用括号对着色。",
		"括号的前景色(4)。需要启用括号对着色。",
		"括号的前景色(5)。需要启用括号对着色。",
		"括号的前景色(6)。需要启用括号对着色。",
		"方括号出现意外的前景色。",
		"非活动括号对指南的背景色(1)。需要启用括号对指南。",
		"非活动括号对指南的背景色(2)。需要启用括号对指南。",
		"非活动括号对指南的背景色(3)。需要启用括号对指南。",
		"非活动括号对指南的背景色(4)。需要启用括号对指南。",
		"非活动括号对指南的背景色(5)。需要启用括号对指南。",
		"非活动括号对指南的背景色(6)。需要启用括号对指南。",
		"活动括号对指南的背景色(1)。需要启用括号对指南。",
		"活动括号对指南的背景色(2)。需要启用括号对指南。",
		"活动括号对指南的背景色(3)。需要启用括号对指南。",
		"活动括号对指南的背景色(4)。需要启用括号对指南。",
		"活动括号对指南的背景色(5)。需要启用括号对指南。",
		"活动括号对指南的背景色(6)。需要启用括号对指南。",
		"用于突出显示 Unicode 字符的边框颜色。",
	],
	"vs/editor/common/editorContextKeys": [
		"编辑器文本是否具有焦点(光标是否闪烁)",
		"编辑器或编辑器小组件是否具有焦点(例如焦点在“查找”小组件中)",
		"编辑器或 RTF 输入是否有焦点(光标是否闪烁)",
		"编辑器是否为只读",
		"上下文是否为差异编辑器",
		"是否已启用 \"editor.columnSelection\"",
		"编辑器是否已选定文本",
		"编辑器是否有多个选择",
		"\"Tab\" 是否将焦点移出编辑器",
		"编辑器软键盘是否可见",
		"该编辑器是否是更大的编辑器(例如笔记本)的一部分",
		"编辑器的语言标识符",
		"编辑器是否具有补全项提供程序",
		"编辑器是否具有代码操作提供程序",
		"编辑器是否具有 CodeLens 提供程序",
		"编辑器是否具有定义提供程序",
		"编辑器是否具有声明提供程序",
		"编辑器是否具有实现提供程序",
		"编辑器是否具有类型定义提供程序",
		"编辑器是否具有悬停提供程序",
		"编辑器是否具有文档突出显示提供程序",
		"编辑器是否具有文档符号提供程序",
		"编辑器是否具有引用提供程序",
		"编辑器是否具有重命名提供程序",
		"编辑器是否具有签名帮助提供程序",
		"编辑器是否具有内联提示提供程序",
		"编辑器是否具有文档格式设置提供程序",
		"编辑器是否具有文档选择格式设置提供程序",
		"编辑器是否具有多个文档格式设置提供程序",
		"编辑器是否有多个文档选择格式设置提供程序",
	],
	"vs/editor/common/languages/modesRegistry": [
		"纯文本",
	],
	"vs/editor/common/model/editStack": [
		"输入",
	],
	"vs/editor/common/standaloneStrings": [
		"无选择",
		"行 {0}, 列 {1} (选中 {2})",
		"行 {0}, 列 {1}",
		"{0} 选择(已选择 {1} 个字符)",
		"{0} 选择",
		"现在将 \"辅助功能支持\" 设置更改为 \"打开\"。",
		"现在正在打开“编辑器辅助功能”文档页。",
		"在差异编辑器的只读窗格中。",
		"在一个差异编辑器的窗格中。",
		"在只读代码编辑器中",
		"在代码编辑器中",
		"若要配置编辑器，将其进行优化以最好地配合屏幕阅读器的使用，请立即按 Command+E。",
		"若要配置编辑器，将其进行优化以最高效地配合屏幕阅读器的使用，按下 Ctrl+E。",
		"配置编辑器，将其进行优化以最好地配合屏幕读取器的使用。",
		"编辑器被配置为永远不进行优化以配合屏幕读取器的使用, 而当前不是这种情况。",
		"在当前编辑器中按 Tab 会将焦点移动到下一个可聚焦的元素。通过按 {0} 切换此行为。",
		"在当前编辑器中按 Tab 会将焦点移动到下一个可聚焦的元素。当前无法通过按键绑定触发命令 {0}。",
		"在当前编辑器中按 Tab 将插入制表符。通过按 {0} 切换此行为。",
		"在当前编辑器中按 Tab 会插入制表符。当前无法通过键绑定触发命令 {0}。",
		"现在按 Command+H 打开一个浏览器窗口, 其中包含有关编辑器辅助功能的详细信息。",
		"现在按 Ctrl+H 打开一个浏览器窗口, 其中包含有关编辑器辅助功能的更多信息。",
		"你可以按 Esc 或 Shift+Esc 消除此工具提示并返回到编辑器。",
		"显示辅助功能帮助",
		"开发人员: 检查令牌",
		"转到行/列...",
		"显示所有快速访问提供程序",
		"命令面板",
		"显示并运行命令",
		"转到符号...",
		"按类别转到符号...",
		"编辑器内容",
		"按 Alt+F1 可打开辅助功能选项。",
		"切换高对比度主题",
		"在 {1} 个文件中进行了 {0} 次编辑",
	],
	"vs/editor/contrib/anchorSelect/browser/anchorSelect": [
		"选择定位点",
		"定位点设置为 {0}:{1}",
		"设置选择定位点",
		"转到选择定位点",
		"选择从定位点到光标",
		"取消选择定位点",
	],
	"vs/editor/contrib/bracketMatching/browser/bracketMatching": [
		"概览标尺上表示匹配括号的标记颜色。",
		"转到括号",
		"选择括号所有内容",
		"转到括号(&&B)",
	],
	"vs/editor/contrib/caretOperations/browser/caretOperations": [
		"向左移动所选文本",
		"向右移动所选文本",
	],
	"vs/editor/contrib/caretOperations/browser/transpose": [
		"转置字母",
	],
	"vs/editor/contrib/clipboard/browser/clipboard": [
		"剪切(&&T)",
		"剪切",
		"剪切",
		"剪切",
		"复制(&&C)",
		"复制",
		"复制",
		"复制",
		"复制为",
		"复制为",
		"粘贴(&&P)",
		"粘贴",
		"粘贴",
		"粘贴",
		"复制并突出显示语法",
	],
	"vs/editor/contrib/codeAction/browser/codeActionCommands": [
		"要运行的代码操作的种类。",
		"控制何时应用返回的操作。",
		"始终应用第一个返回的代码操作。",
		"如果仅返回的第一个代码操作，则应用该操作。",
		"不要应用返回的代码操作。",
		"如果只应返回首选代码操作，则应返回控件。",
		"应用代码操作时发生未知错误",
		"快速修复...",
		"没有可用的代码操作",
		"没有适用于\"{0}\"的首选代码操作",
		"没有适用于\"{0}\"的代码操作",
		"没有可用的首选代码操作",
		"没有可用的代码操作",
		"重构...",
		"没有适用于\"{0}\"的首选重构",
		"没有可用的\"{0}\"重构",
		"没有可用的首选重构",
		"没有可用的重构操作",
		"源代码操作...",
		"没有适用于\"{0}\"的首选源操作",
		"没有适用于“ {0}”的源操作",
		"没有可用的首选源操作",
		"没有可用的源代码操作",
		"整理 import 语句",
		"没有可用的整理 import 语句操作",
		"全部修复",
		"没有可用的“全部修复”操作",
		"自动修复...",
		"没有可用的自动修复程序",
	],
	"vs/editor/contrib/codeAction/browser/lightBulbWidget": [
		"显示代码操作。首选可用的快速修复({0})",
		"显示代码操作({0})",
		"显示代码操作",
	],
	"vs/editor/contrib/codelens/browser/codelensController": [
		"显示当前行的 Code Lens 命令",
	],
	"vs/editor/contrib/colorPicker/browser/colorPickerWidget": [
		"单击以切换颜色选项 (rgb/hsl/hex)",
	],
	"vs/editor/contrib/comment/browser/comment": [
		"切换行注释",
		"切换行注释(&&T)",
		"添加行注释",
		"删除行注释",
		"切换块注释",
		"切换块注释(&&B)",
	],
	"vs/editor/contrib/contextmenu/browser/contextmenu": [
		"显示编辑器上下文菜单",
	],
	"vs/editor/contrib/cursorUndo/browser/cursorUndo": [
		"光标撤消",
		"光标重做",
	],
	"vs/editor/contrib/editorState/browser/keybindingCancellation": [
		"编辑器是否运行可取消的操作，例如“预览引用”",
	],
	"vs/editor/contrib/find/browser/findController": [
		"查找",
		"查找(&&F)",
		"重写“使用正则表达式”标记。\r\n将不会保留该标记供将来使用。\r\n0: 不执行任何操作\r\n1: True\r\n2: False",
		"重写“匹配整个字词”标记。\r\n将不会保留该标记供将来使用。\r\n0: 不执行任何操作\r\n1: True\r\n2: False",
		"重写“数学案例”标记。\r\n将不会保留该标记供将来使用。\r\n0: 不执行任何操作\r\n1: True\r\n2: False",
		"重写“保留服务案例”标记。\r\n将不会保留该标记供将来使用。\r\n0: 不执行任何操作\r\n1: True\r\n2: False",
		"使用参数查找",
		"查找选定内容",
		"查找下一个",
		"查找上一个",
		"查找下一个选择",
		"查找上一个选择",
		"替换",
		"替换(&&R)",
	],
	"vs/editor/contrib/find/browser/findWidget": [
		"编辑器查找小组件中的“在选定内容中查找”图标。",
		"用于指示编辑器查找小组件已折叠的图标。",
		"用于指示编辑器查找小组件已展开的图标。",
		"编辑器查找小组件中的“替换”图标。",
		"编辑器查找小组件中的“全部替换”图标。",
		"编辑器查找小组件中的“查找上一个”图标。",
		"编辑器查找小组件中的“查找下一个”图标。",
		"查找",
		"查找",
		"上一个匹配项",
		"下一个匹配项",
		"在选定内容中查找",
		"关闭",
		"替换",
		"替换",
		"替换",
		"全部替换",
		"切换替换",
		"仅高亮了前 {0} 个结果，但所有查找操作均针对全文。",
		"{1} 中的 {0}",
		"无结果",
		"找到 {0}",
		"为“{1}”找到 {0}",
		"在 {2} 处找到“{1}”的 {0}",
		"为“{1}”找到 {0}",
		"Ctrl+Enter 现在由全部替换改为插入换行。你可以修改editor.action.replaceAll 的按键绑定以覆盖此行为。",
	],
	"vs/editor/contrib/folding/browser/folding": [
		"可折叠区域的数量限制为最多 {0} 个。增加配置选项[“最大折叠区域数”](command:workbench.action.openSettings?[\"editor.foldingMaximumRegions\"])以启用更多功能。",
		"展开",
		"以递归方式展开",
		"折叠",
		"切换折叠",
		"以递归方式折叠",
		"折叠所有块注释",
		"折叠所有区域",
		"展开所有区域",
		"折叠除所选区域之外的所有区域",
		"展开除所选区域之外的所有区域",
		"全部折叠",
		"全部展开",
		"跳转到父级折叠",
		"转到上一个折叠范围",
		"转到下一个折叠范围",
		"折叠级别 {0}",
		"折叠范围后面的背景颜色。颜色必须设为透明，以免隐藏底层装饰。",
		"编辑器装订线中折叠控件的颜色。",
	],
	"vs/editor/contrib/folding/browser/foldingDecorations": [
		"编辑器字形边距中已展开的范围的图标。",
		"编辑器字形边距中已折叠的范围的图标。",
	],
	"vs/editor/contrib/fontZoom/browser/fontZoom": [
		"放大编辑器字体",
		"缩小编辑器字体",
		"重置编辑器字体大小",
	],
	"vs/editor/contrib/format/browser/format": [
		"在第 {0} 行进行了 1 次格式编辑",
		"在第 {1} 行进行了 {0} 次格式编辑",
		"第 {0} 行到第 {1} 行间进行了 1 次格式编辑",
		"第 {1} 行到第 {2} 行间进行了 {0} 次格式编辑",
	],
	"vs/editor/contrib/format/browser/formatActions": [
		"格式化文档",
		"格式化选定内容",
	],
	"vs/editor/contrib/gotoError/browser/gotoError": [
		"转到下一个问题 (错误、警告、信息)",
		"“转到下一个”标记的图标。",
		"转到上一个问题 (错误、警告、信息)",
		"“转到上一个”标记的图标。",
		"转到文件中的下一个问题 (错误、警告、信息)",
		"下一个问题(&&P)",
		"转到文件中的上一个问题 (错误、警告、信息)",
		"上一个问题(&&P)",
	],
	"vs/editor/contrib/gotoError/browser/gotoErrorWidget": [
		"错误",
		"警告",
		"信息",
		"提示",
		"{1} 中的 {0}",
		"{0} 个问题(共 {1} 个)",
		"{0} 个问题(共 {1} 个)",
		"编辑器标记导航小组件错误颜色。",
		"编辑器标记导航小组件错误标题背景色。",
		"编辑器标记导航小组件警告颜色。",
		"编辑器标记导航小组件警告标题背景色。",
		"编辑器标记导航小组件信息颜色。",
		"编辑器标记导航小组件信息标题背景色。",
		"编辑器标记导航小组件背景色。",
	],
	"vs/editor/contrib/gotoSymbol/browser/goToCommands": [
		"快速查看",
		"定义",
		"未找到“{0}”的任何定义",
		"找不到定义",
		"转到定义",
		"打开侧边的定义",
		"速览定义",
		"声明",
		"未找到“{0}”的声明",
		"未找到声明",
		"转到声明",
		"未找到“{0}”的声明",
		"未找到声明",
		"查看声明",
		"类型定义",
		"未找到“{0}”的类型定义",
		"未找到类型定义",
		"转到类型定义",
		"快速查看类型定义",
		"实现",
		"未找到“{0}”的实现",
		"未找到实现",
		"转到实现",
		"查看实现",
		"未找到\"{0}\"的引用",
		"未找到引用",
		"转到引用",
		"引用",
		"查看引用",
		"引用",
		"转到任何符号",
		"位置",
		"无“{0}”的结果",
		"引用",
		"转到定义(&&D)",
		"转到声明(&&D)",
		"转到类型定义(&&T)",
		"转到实现(&&I)",
		"转到引用(&&R)",
	],
	"vs/editor/contrib/gotoSymbol/browser/link/goToDefinitionAtPosition": [
		"单击显示 {0} 个定义。",
	],
	"vs/editor/contrib/gotoSymbol/browser/peek/referencesController": [
		"引用速览是否可见，例如“速览引用”或“速览定义”",
		"正在加载...",
		"{0} ({1})",
	],
	"vs/editor/contrib/gotoSymbol/browser/peek/referencesTree": [
		"{0} 个引用",
		"{0} 个引用",
		"引用",
	],
	"vs/editor/contrib/gotoSymbol/browser/peek/referencesWidget": [
		"无可用预览",
		"无结果",
		"引用",
	],
	"vs/editor/contrib/gotoSymbol/browser/referencesModel": [
		"在文件 {0} 的 {1} 行 {2} 列的符号",
		"{0} 中 {1} 行 {2} 列的符号，{3}",
		"{0} 中有 1 个符号，完整路径: {1}",
		"{1} 中有 {0} 个符号，完整路径: {2}",
		"未找到结果",
		"在 {0} 中找到 1 个符号",
		"在 {1} 中找到 {0} 个符号",
		"在 {1} 个文件中找到 {0} 个符号",
	],
	"vs/editor/contrib/gotoSymbol/browser/symbolNavigation": [
		"是否存在只能通过键盘导航的符号位置。",
		"{1} 的符号 {0}，下一个使用 {2}",
		"{1} 的符号 {0}",
	],
	"vs/editor/contrib/hover/browser/hover": [
		"显示悬停",
		"显示定义预览悬停",
	],
	"vs/editor/contrib/hover/browser/markdownHoverParticipant": [
		"正在加载...",
		"出于性能原因，未对长行进行解析。解析长度阈值可通过“editor.maxTokenizationLineLength”进行配置。",
	],
	"vs/editor/contrib/hover/browser/markerHoverParticipant": [
		"查看问题",
		"没有可用的快速修复",
		"正在检查快速修复...",
		"没有可用的快速修复",
		"快速修复...",
	],
	"vs/editor/contrib/inPlaceReplace/browser/inPlaceReplace": [
		"替换为上一个值",
		"替换为下一个值",
	],
	"vs/editor/contrib/indentation/browser/indentation": [
		"将缩进转换为空格",
		"将缩进转换为制表符",
		"已配置制表符大小",
		"选择当前文件的制表符大小",
		"使用 \"Tab\" 缩进",
		"使用空格缩进",
		"从内容中检测缩进方式",
		"重新缩进行",
		"重新缩进所选行",
	],
	"vs/editor/contrib/inlineCompletions/browser/ghostTextController": [
		"内联建议是否可见",
		"内联建议是否以空白开头",
		"内联建议是否以小于选项卡插入内容的空格开头",
		"显示下一个内联建议",
		"显示上一个内联建议",
		"触发内联建议",
	],
	"vs/editor/contrib/inlineCompletions/browser/inlineCompletionsHoverParticipant": [
		"下一个",
		"上一个",
		"接受",
		"建议:",
	],
	"vs/editor/contrib/lineSelection/browser/lineSelection": [
		"展开行选择",
	],
	"vs/editor/contrib/linesOperations/browser/linesOperations": [
		"向上复制行",
		"向上复制一行(&&C)",
		"向下复制行",
		"向下复制一行(&&P)",
		"重复选择",
		"重复选择(&&D)",
		"向上移动行",
		"向上移动一行(&&V)",
		"向下移动行",
		"向下移动一行(&&L)",
		"按升序排列行",
		"按降序排列行",
		"删除重复行",
		"裁剪尾随空格",
		"删除行",
		"行缩进",
		"行减少缩进",
		"在上面插入行",
		"在下面插入行",
		"删除左侧所有内容",
		"删除右侧所有内容",
		"合并行",
		"转置光标处的字符",
		"转换为大写",
		"转换为小写",
		"转换为词首字母大写",
		"转换为蛇形命名法",
	],
	"vs/editor/contrib/linkedEditing/browser/linkedEditing": [
		"启动链接编辑",
		"编辑器根据类型自动重命名时的背景色。",
	],
	"vs/editor/contrib/links/browser/links": [
		"执行命令",
		"打开链接",
		"cmd + 单击",
		"ctrl + 单击",
		"option + 单击",
		"alt + 单击",
		"执行命令 {0}",
		"此链接格式不正确，无法打开: {0}",
		"此链接目标已丢失，无法打开。",
		"打开链接",
	],
	"vs/editor/contrib/message/browser/messageController": [
		"编辑器当前是否正在显示内联消息",
		"无法在只读编辑器中编辑",
	],
	"vs/editor/contrib/multicursor/browser/multicursor": [
		"添加的光标: {0}",
		"添加的游标: {0}",
		"在上面添加光标",
		"在上面添加光标(&&A)",
		"在下面添加光标",
		"在下面添加光标(&&D)",
		"在行尾添加光标",
		"在行尾添加光标(&&U)",
		"在底部添加光标",
		"在顶部添加光标",
		"将下一个查找匹配项添加到选择",
		"添加下一个匹配项(&&N)",
		"将选择内容添加到上一查找匹配项",
		"添加上一个匹配项(&&R)",
		"将上次选择移动到下一个查找匹配项",
		"将上个选择内容移动到上一查找匹配项",
		"选择所有找到的查找匹配项",
		"选择所有匹配项(&&O)",
		"更改所有匹配项",
	],
	"vs/editor/contrib/parameterHints/browser/parameterHints": [
		"触发参数提示",
	],
	"vs/editor/contrib/parameterHints/browser/parameterHintsWidget": [
		"“显示下一个参数”提示的图标。",
		"“显示上一个参数”提示的图标。",
		"{0}，提示",
		"参数提示中活动项的前景色。",
	],
	"vs/editor/contrib/peekView/browser/peekView": [
		"速览中是否嵌入了当前代码编辑器",
		"关闭",
		"速览视图标题区域背景颜色。",
		"速览视图标题颜色。",
		"速览视图标题信息颜色。",
		"速览视图边框和箭头颜色。",
		"速览视图结果列表背景色。",
		"速览视图结果列表中行节点的前景色。",
		"速览视图结果列表中文件节点的前景色。",
		"速览视图结果列表中所选条目的背景色。",
		"速览视图结果列表中所选条目的前景色。",
		"速览视图编辑器背景色。",
		"速览视图编辑器中装订线的背景色。",
		"在速览视图结果列表中匹配突出显示颜色。",
		"在速览视图编辑器中匹配突出显示颜色。",
		"在速览视图编辑器中匹配项的突出显示边框。",
	],
	"vs/editor/contrib/quickAccess/browser/gotoLineQuickAccess": [
		"先打开文本编辑器然后跳转到行。",
		"转到第 {0} 行第 {1} 个字符。",
		"转到行 {0}。",
		"当前行: {0}，字符: {1}。键入要导航到的行号(介于 1 至 {2} 之间)。",
		"当前行: {0}，字符: {1}。 键入要导航到的行号。",
	],
	"vs/editor/contrib/quickAccess/browser/gotoSymbolQuickAccess": [
		"要转到符号，首先打开具有符号信息的文本编辑器。",
		"活动文本编辑器不提供符号信息。",
		"没有匹配的编辑器符号",
		"没有编辑器符号",
		"在侧边打开",
		"在底部打开",
		"符号({0})",
		"属性({0})",
		"方法({0})",
		"函数({0})",
		"构造函数 ({0})",
		"变量({0})",
		"类({0})",
		"结构({0})",
		"事件({0})",
		"运算符({0})",
		"接口({0})",
		"命名空间({0})",
		"包({0})",
		"类型参数({0})",
		"模块({0})",
		"属性({0})",
		"枚举({0})",
		"枚举成员({0})",
		"字符串({0})",
		"文件({0})",
		"数组({0})",
		"数字({0})",
		"布尔值({0})",
		"对象({0})",
		"键({0})",
		"字段({0})",
		"常量({0})",
	],
	"vs/editor/contrib/rename/browser/rename": [
		"无结果。",
		"解析重命名位置时发生未知错误",
		"正在重命名“{0}”",
		"重命名 {0}",
		"成功将“{0}”重命名为“{1}”。摘要: {2}",
		"重命名无法应用修改",
		"重命名无法计算修改",
		"重命名符号",
		"启用/禁用重命名之前预览更改的功能",
	],
	"vs/editor/contrib/rename/browser/renameInputField": [
		"重命名输入小组件是否可见",
		"重命名输入。键入新名称并按 \"Enter\" 提交。",
		"按 {0} 进行重命名，按 {1} 进行预览",
	],
	"vs/editor/contrib/smartSelect/browser/smartSelect": [
		"展开选择",
		"扩大选区(&&E)",
		"收起选择",
		"缩小选区(&&S)",
	],
	"vs/editor/contrib/snippet/browser/snippetController2": [
		"编辑器目前是否在代码片段模式下",
		"在代码片段模式下时是否存在下一制表位",
		"在代码片段模式下时是否存在上一制表位",
	],
	"vs/editor/contrib/snippet/browser/snippetVariables": [
		"星期天",
		"星期一",
		"星期二",
		"星期三",
		"星期四",
		"星期五",
		"星期六",
		"周日",
		"周一",
		"周二",
		"周三",
		"周四",
		"周五",
		"周六",
		"一月",
		"二月",
		"三月",
		"四月",
		"5月",
		"六月",
		"七月",
		"八月",
		"九月",
		"十月",
		"十一月",
		"十二月",
		"1月",
		"2月",
		"3月",
		"4月",
		"5月",
		"6月",
		"7月",
		"8月",
		"9月",
		"10月",
		"11 月",
		"12月",
	],
	"vs/editor/contrib/suggest/browser/suggest": [
		"建议是否可见",
		"建议详细信息是否可见",
		"是否存在多条建议可供选择",
		"插入当前建议是否会导致更改或导致已键入所有内容",
		"按 Enter 时是否会插入建议",
		"当前建议是否具有插入和替换行为",
		"默认行为是否是插入或替换",
		"当前建议是否支持解析更多详细信息",
	],
	"vs/editor/contrib/suggest/browser/suggestController": [
		"选择“{0}”后进行了其他 {1} 次编辑",
		"触发建议",
		"插入",
		"插入",
		"替换",
		"替换",
		"插入",
		"显示更少",
		"显示更多",
		"重置建议小组件大小",
	],
	"vs/editor/contrib/suggest/browser/suggestWidget": [
		"建议小组件的背景色。",
		"建议小组件的边框颜色。",
		"建议小组件的前景色。",
		"建议小组件中所选条目的前景色。",
		"建议小组件中所选条目的图标前景色。",
		"建议小组件中所选条目的背景色。",
		"建议小组件中匹配内容的高亮颜色。",
		"当某项获得焦点时，在建议小组件中突出显示的匹配项的颜色。",
		"建议小组件状态的前景色。",
		"正在加载...",
		"无建议。",
		"建议",
		"{0}{1}，{2}",
		"{0}{1}",
		"{0}，{1}",
		"{0}，文档: {1}",
	],
	"vs/editor/contrib/suggest/browser/suggestWidgetDetails": [
		"关闭",
		"正在加载…",
	],
	"vs/editor/contrib/suggest/browser/suggestWidgetRenderer": [
		"建议小组件中的详细信息的图标。",
		"了解详细信息",
	],
	"vs/editor/contrib/suggest/browser/suggestWidgetStatus": [
		"{0} ({1})",
	],
	"vs/editor/contrib/symbolIcons/browser/symbolIcons": [
		"数组符号的前景色。这些符号将显示在大纲、痕迹导航栏和建议小组件中。",
		"布尔符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"类符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"颜色符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"常量符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"构造函数符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"枚举符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"枚举器成员符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"事件符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"字段符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"文件符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"文件夹符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"函数符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"接口符号的前景色。这些符号将显示在大纲、痕迹导航栏和建议小组件中。",
		"键符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"关键字符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"方法符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"模块符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"命名空间符号的前景颜色。这些符号出现在轮廓、痕迹导航栏和建议小部件中。",
		"空符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"数字符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"对象符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"运算符符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"包符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"属性符号的前景色。这些符号出现在大纲、痕迹导航栏和建议小组件中。",
		"参考符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"片段符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"字符串符号的前景颜色。这些符号出现在轮廓、痕迹导航栏和建议小部件中。",
		"结构符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"文本符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"类型参数符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"单位符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
		"变量符号的前景颜色。这些符号出现在大纲、痕迹导航栏和建议小部件中。",
	],
	"vs/editor/contrib/toggleTabFocusMode/browser/toggleTabFocusMode": [
		"切换 Tab 键移动焦点",
		"Tab 键将移动到下一可聚焦的元素",
		"Tab 键将插入制表符",
	],
	"vs/editor/contrib/tokenization/browser/tokenization": [
		"开发人员: 强制重新进行标记",
	],
	"vs/editor/contrib/unicodeHighlighter/browser/unicodeHighlighter": [
		"扩展编辑器中随警告消息一同显示的图标。",
		"本文档包含许多非基本 ASCII unicode 字符",
		"本文档包含许多不明确的 unicode 字符",
		"本文档包含许多不可见的 unicode 字符",
		"字符 {0} 可能会与字符 {1} 混淆，后者在源代码中更为常见。",
		"字符 {0} 不可见。",
		"字符 {0} 不是基本 ASCII 字符。",
		"调整设置",
		"禁用批注中的突出显示",
		"禁用批注中字符的突出显示",
		"禁用字符串中的突出显示",
		"禁用字符串中字符的突出显示",
		"禁用不明确的突出显示",
		"禁止突出显示歧义字符",
		"禁用不可见突出显示",
		"禁止突出显示不可见字符",
		"禁用非 ASCII 突出显示",
		"禁止突出显示非基本 ASCII 字符",
		"显示排除选项",
		"不突出显示 {0} (不可见字符)",
		"在突出显示内容中排除{0}",
		"允许语言“{0}”中更常见的 unicode 字符。",
		"配置 Unicode 突出显示选项",
	],
	"vs/editor/contrib/unusualLineTerminators/browser/unusualLineTerminators": [
		"异常行终止符",
		"检测到异常行终止符",
		"文件“{0}”包含一个或多个异常的行终止符，例如行分隔符(LS)或段落分隔符(PS)。\r\n\r\n建议从文件中删除它们。可通过“editor.unusualLineTerminators”进行配置。",
		"删除异常行终止符",
		"忽略",
	],
	"vs/editor/contrib/wordHighlighter/browser/wordHighlighter": [
		"读取访问期间符号的背景色，例如读取变量时。颜色必须透明，以免隐藏下面的修饰效果。",
		"写入访问过程中符号的背景色，例如写入变量时。颜色必须透明，以免隐藏下面的修饰效果。",
		"符号在进行读取访问操作时的边框颜色，例如读取变量。",
		"符号在进行写入访问操作时的边框颜色，例如写入变量。",
		"用于突出显示符号的概述标尺标记颜色。颜色必须透明，以免隐藏下面的修饰效果。",
		"用于突出显示写权限符号的概述标尺标记颜色。颜色必须透明，以免隐藏下面的修饰效果。",
		"转到下一个突出显示的符号",
		"转到上一个突出显示的符号",
		"触发符号高亮",
	],
	"vs/editor/contrib/wordOperations/browser/wordOperations": [
		"删除 Word",
	],
	"vs/platform/actions/browser/menuEntryActionViewItem": [
		"{0} ({1})",
		"{0} ({1})",
	],
	"vs/platform/configuration/common/configurationRegistry": [
		"默认语言配置替代",
		"配置要为 {0} 语言替代的设置。",
		"针对某种语言，配置替代编辑器设置。",
		"此设置不支持按语言配置。",
		"针对某种语言，配置替代编辑器设置。",
		"此设置不支持按语言配置。",
		"无法注册空属性",
		"无法注册“{0}”。其符合描述特定语言编辑器设置的表达式 \"\\\\[.*\\\\]$\"。请使用 \"configurationDefaults\"。",
		"无法注册“{0}”。此属性已注册。",
	],
	"vs/platform/contextkey/browser/contextKeyService": [
		"用于返回上下文键的相关信息的命令",
	],
	"vs/platform/contextkey/common/contextkeys": [
		"操作系统是否 macOS",
		"操作系统是否为 Linux",
		"操作系统是否为 Windows",
		"平台是否为 Web 浏览器",
		"操作系统是否是非浏览器平台上的 macOS",
		"操作系统是否为 IOS",
		"键盘焦点是否在输入框中",
	],
	"vs/platform/keybinding/common/abstractKeybindingService": [
		"({0})已按下。正在等待按下第二个键...",
		"组合键({0}，{1})不是命令。",
	],
	"vs/platform/list/browser/listService": [
		"工作台",
		"映射为 `Ctrl` (Windows 和 Linux) 或 `Command` (macOS)。",
		"映射为 `Alt` (Windows 和 Linux) 或 `Option` (macOS)。",
		"在通过鼠标多选树和列表条目时使用的修改键 (例如“资源管理器”、“打开的编辑器”和“源代码管理”视图)。“在侧边打开”功能所需的鼠标动作 (若可用) 将会相应调整，不与多选修改键冲突。",
		"控制如何使用鼠标打开树和列表中的项(若支持)。请注意，如果此设置不适用，某些树和列表可能会选择忽略它。",
		"控制列表和树是否支持工作台中的水平滚动。警告: 打开此设置影响会影响性能。",
		"控制树缩进(以像素为单位)。",
		"控制树是否应呈现缩进参考线。",
		"控制列表和树是否具有平滑滚动效果。",
		"对鼠标滚轮滚动事件的 `deltaX` 和 `deltaY` 乘上的系数。",
		"按下\"Alt\"时滚动速度倍增。",
		"简单键盘导航聚焦与键盘输入相匹配的元素。仅对前缀进行匹配。",
		"高亮键盘导航会突出显示与键盘输入相匹配的元素。进一步向上和向下导航将仅遍历突出显示的元素。",
		"筛选器键盘导航将筛选出并隐藏与键盘输入不匹配的所有元素。",
		"控制工作台中的列表和树的键盘导航样式。它可为“简单”、“突出显示”或“筛选”。",
		"控制列表和树中的键盘导航是否仅通过键入自动触发。如果设置为 `false` ，键盘导航只在执行 `list.toggleKeyboardNavigation` 命令时触发，您可以为该命令指定键盘快捷方式。",
		"控制在单击文件夹名称时如何扩展树文件夹。请注意，如果不适用，某些树和列表可能会选择忽略此设置。",
	],
	"vs/platform/markers/common/markers": [
		"错误",
		"警告",
		"信息",
	],
	"vs/platform/quickinput/browser/commandsQuickAccess": [
		"{0}, {1}",
		"最近使用",
		"其他命令",
		"命令\"{0}\"导致错误 ({1})",
	],
	"vs/platform/quickinput/browser/helpQuickAccess": [
		"全局命令",
		"编辑器命令",
		"{0}, {1}",
	],
	"vs/platform/theme/common/colorRegistry": [
		"整体前景色。此颜色仅在不被组件覆盖时适用。",
		"错误信息的整体前景色。此颜色仅在不被组件覆盖时适用。",
		"提供其他信息的说明文本的前景色，例如标签文本。",
		"工作台中图标的默认颜色。",
		"焦点元素的整体边框颜色。此颜色仅在不被其他组件覆盖时适用。",
		"在元素周围额外的一层边框，用来提高对比度从而区别其他元素。",
		"在活动元素周围额外的一层边框，用来提高对比度从而区别其他元素。",
		"工作台所选文本的背景颜色(例如输入字段或文本区域)。注意，本设置不适用于编辑器。",
		"文字分隔符的颜色。",
		"文本中链接的前景色。",
		"文本中链接在点击或鼠标悬停时的前景色 。",
		"预格式化文本段的前景色。",
		"文本中块引用的背景颜色。",
		"文本中块引用的边框颜色。",
		"文本中代码块的背景颜色。",
		"编辑器内小组件(如查找/替换)的阴影颜色。",
		"输入框背景色。",
		"输入框前景色。",
		"输入框边框。",
		"输入字段中已激活选项的边框颜色。",
		"输入字段中激活选项的背景颜色。",
		"输入字段中选项的背景悬停颜色。",
		"输入字段中已激活的选项的前景色。",
		"输入框中占位符的前景色。",
		"输入验证结果为信息级别时的背景色。",
		"输入验证结果为信息级别时的前景色。",
		"严重性为信息时输入验证的边框颜色。",
		"严重性为警告时输入验证的背景色。",
		"输入验证结果为警告级别时的前景色。",
		"严重性为警告时输入验证的边框颜色。",
		"输入验证结果为错误级别时的背景色。",
		"输入验证结果为错误级别时的前景色。",
		"严重性为错误时输入验证的边框颜色。",
		"下拉列表背景色。",
		"下拉列表背景色。",
		"下拉列表前景色。",
		"下拉列表边框。",
		"复选框小部件的背景颜色。",
		"复选框小部件的前景色。",
		"复选框小部件的边框颜色。",
		"按钮前景色。",
		"按钮背景色。",
		"按钮在悬停时的背景颜色。",
		"按钮边框颜色。",
		"辅助按钮前景色。",
		"辅助按钮背景色。",
		"悬停时的辅助按钮背景色。",
		"Badge 背景色。Badge 是小型的信息标签，如表示搜索结果数量的标签。",
		"Badge 前景色。Badge 是小型的信息标签，如表示搜索结果数量的标签。",
		"表示视图被滚动的滚动条阴影。",
		"滚动条滑块背景色",
		"滚动条滑块在悬停时的背景色",
		"滚动条滑块在被点击时的背景色。",
		"表示长时间操作的进度条的背景色。",
		"编辑器中错误文本的背景色。颜色必须透明，以免隐藏下面的修饰效果。",
		"编辑器中错误波浪线的前景色。",
		"编辑器中错误框的边框颜色。",
		"编辑器中警告文本的背景色。颜色必须透明，以免隐藏下面的修饰效果。",
		"编辑器中警告波浪线的前景色。",
		"编辑器中警告框的边框颜色。",
		"编辑器中信息文本的背景色。颜色必须透明，以免隐藏下面的修饰效果。",
		"编辑器中信息波浪线的前景色。",
		"编辑器中信息框的边框颜色。",
		"编辑器中提示波浪线的前景色。",
		"编辑器中提示框的边框颜色。",
		"活动框格的边框颜色。",
		"编辑器背景色。",
		"编辑器默认前景色。",
		"编辑器组件(如查找/替换)背景颜色。",
		"编辑器小部件的前景色，如查找/替换。",
		"编辑器小部件的边框颜色。此颜色仅在小部件有边框且不被小部件重写时适用。",
		"编辑器小部件大小调整条的边框颜色。此颜色仅在小部件有调整边框且不被小部件颜色覆盖时使用。",
		"背景颜色快速选取器。快速选取器小部件是选取器(如命令调色板)的容器。",
		"前景颜色快速选取器。快速选取器小部件是命令调色板等选取器的容器。",
		"标题背景颜色快速选取器。快速选取器小部件是命令调色板等选取器的容器。",
		"快速选取器分组标签的颜色。",
		"快速选取器分组边框的颜色。",
		"键绑定标签背景色。键绑定标签用于表示键盘快捷方式。",
		"键绑定标签前景色。键绑定标签用于表示键盘快捷方式。",
		"键绑定标签边框色。键绑定标签用于表示键盘快捷方式。",
		"键绑定标签边框底部色。键绑定标签用于表示键盘快捷方式。",
		"编辑器所选内容的颜色。",
		"用以彰显高对比度的所选文本的颜色。",
		"非活动编辑器中所选内容的颜色，颜色必须透明，以免隐藏下面的装饰效果。",
		"具有与所选项相关内容的区域的颜色。颜色必须透明，以免隐藏下面的修饰效果。",
		"与所选项内容相同的区域的边框颜色。",
		"当前搜索匹配项的颜色。",
		"其他搜索匹配项的颜色。颜色必须透明，以免隐藏下面的修饰效果。",
		"限制搜索范围的颜色。颜色不能不透明，以免隐藏底层装饰。",
		"当前搜索匹配项的边框颜色。",
		"其他搜索匹配项的边框颜色。",
		"限制搜索的范围的边框颜色。颜色必须透明，以免隐藏下面的修饰效果。",
		"搜索编辑器查询匹配的颜色。",
		"搜索编辑器查询匹配的边框颜色。",
		"在下面突出显示悬停的字词。颜色必须透明，以免隐藏下面的修饰效果。",
		"编辑器悬停提示的背景颜色。",
		"编辑器悬停的前景颜色。",
		"光标悬停时编辑器的边框颜色。",
		"编辑器悬停状态栏的背景色。",
		"活动链接颜色。",
		"内联提示的前景色",
		"内联提示的背景色",
		"类型内联提示的前景色",
		"类型内联提示的背景色",
		"参数内联提示的前景色",
		"参数内联提示的背景色",
		"用于灯泡操作图标的颜色。",
		"用于灯泡自动修复操作图标的颜色。",
		"已插入的文本的背景色。颜色必须透明，以免隐藏下面的修饰效果。",
		"已删除的文本的背景色。颜色必须透明，以免隐藏下面的修饰效果。",
		"插入的文本的轮廓颜色。",
		"被删除文本的轮廓颜色。",
		"两个文本编辑器之间的边框颜色。",
		"差异编辑器的对角线填充颜色。对角线填充用于并排差异视图。",
		"焦点项在列表或树活动时的背景颜色。活动的列表或树具有键盘焦点，非活动的没有。",
		"焦点项在列表或树活动时的前景颜色。活动的列表或树具有键盘焦点，非活动的没有。",
		"列表/树活动时，焦点项目的列表/树边框色。活动的列表/树具有键盘焦点，非活动的没有。",
		"已选项在列表或树活动时的背景颜色。活动的列表或树具有键盘焦点，非活动的没有。",
		"已选项在列表或树活动时的前景颜色。活动的列表或树具有键盘焦点，非活动的没有。",
		"已选项在列表/树活动时的列表/树图标前景颜色。活动的列表/树具有键盘焦点，非活动的则没有。",
		"已选项在列表或树非活动时的背景颜色。活动的列表或树具有键盘焦点，非活动的没有。",
		"已选项在列表或树非活动时的前景颜色。活动的列表或树具有键盘焦点，非活动的没有。",
		"已选项在列表/树非活动时的图标前景颜色。活动的列表/树具有键盘焦点，非活动的则没有。",
		"非活动的列表或树控件中焦点项的背景颜色。活动的列表或树具有键盘焦点，非活动的没有。",
		"列表/数非活动时，焦点项目的列表/树边框色。活动的列表/树具有键盘焦点，非活动的没有。",
		"使用鼠标移动项目时，列表或树的背景颜色。",
		"鼠标在项目上悬停时，列表或树的前景颜色。",
		"使用鼠标移动项目时，列表或树进行拖放的背景颜色。",
		"在列表或树中搜索时，其中匹配内容的高亮颜色。",
		"在列表或树中搜索时，匹配活动聚焦项的突出显示内容的列表/树前景色。",
		"列表或树中无效项的前景色，例如资源管理器中没有解析的根目录。",
		"包含错误的列表项的前景颜色。",
		"包含警告的列表项的前景颜色。",
		"列表和树中类型筛选器小组件的背景色。",
		"列表和树中类型筛选器小组件的轮廓颜色。",
		"当没有匹配项时，列表和树中类型筛选器小组件的轮廓颜色。",
		"筛选后的匹配项的背景颜色。",
		"筛选后的匹配项的边框颜色。",
		"缩进参考线的树描边颜色。",
		"列之间的表边框颜色。",
		"奇数表行的背景色。",
		"取消强调的项目的列表/树前景颜色。",
		"请改用 quickInputList.focusBackground",
		"焦点项目的快速选择器前景色。",
		"焦点项目的快速选取器图标前景色。",
		"焦点项目的快速选择器背景色。",
		"菜单的边框颜色。",
		"菜单项的前景颜色。",
		"菜单项的背景颜色。",
		"菜单中选定菜单项的前景色。",
		"菜单中所选菜单项的背景色。",
		"菜单中所选菜单项的边框颜色。",
		"菜单中分隔线的颜色。",
		"使用鼠标悬停在操作上时显示工具栏背景",
		"使用鼠标悬停在操作上时显示工具栏轮廓",
		"将鼠标悬停在操作上时的工具栏背景",
		"代码片段 Tab 位的高亮背景色。",
		"代码片段 Tab 位的高亮边框颜色。",
		"代码片段中最后的 Tab 位的高亮背景色。",
		"代码片段中最后的制表位的高亮边框颜色。",
		"焦点导航路径的颜色",
		"导航路径项的背景色。",
		"焦点导航路径的颜色",
		"已选导航路径项的颜色。",
		"导航路径项选择器的背景色。",
		"当前标题背景的内联合并冲突。颜色不能不透明，以免隐藏底层装饰。",
		"内联合并冲突中的当前内容背景。颜色必须透明，以免隐藏下面的修饰效果。",
		"内联合并冲突中的传入标题背景。颜色必须透明，以免隐藏下面的修饰效果。",
		"内联合并冲突中的传入内容背景。颜色必须透明，以免隐藏下面的修饰效果。",
		"内联合并冲突中的常见祖先标头背景。颜色必须透明，以免隐藏下面的修饰效果。",
		"内联合并冲突中的常见祖先内容背景。颜色必须透明，以免隐藏下面的修饰效果。",
		"内联合并冲突中标头和分割线的边框颜色。",
		"内联合并冲突中当前版本区域的概览标尺前景色。",
		"内联合并冲突中传入的版本区域的概览标尺前景色。",
		"内联合并冲突中共同祖先区域的概览标尺前景色。",
		"用于查找匹配项的概述标尺标记颜色。颜色必须透明，以免隐藏下面的修饰效果。",
		"用于突出显示所选内容的概述标尺标记颜色。颜色必须透明，以免隐藏下面的修饰效果。",
		"用于查找匹配项的迷你地图标记颜色。",
		"用于重复编辑器选择的缩略图标记颜色。",
		"编辑器选区在迷你地图中对应的标记颜色。",
		"用于错误的迷你地图标记颜色。",
		"用于警告的迷你地图标记颜色。",
		"迷你地图背景颜色。",
		"在缩略图中呈现的前景元素的不透明度。例如，\"#000000c0\" 将呈现不透明度为 75% 的元素。",
		"迷你地图滑块背景颜色。",
		"悬停时，迷你地图滑块的背景颜色。",
		"单击时，迷你地图滑块的背景颜色。",
		"用于问题错误图标的颜色。",
		"用于问题警告图标的颜色。",
		"用于问题信息图标的颜色。",
		"图表中使用的前景颜色。",
		"用于图表中的水平线条的颜色。",
		"图表可视化效果中使用的红色。",
		"图表可视化效果中使用的蓝色。",
		"图表可视化效果中使用的黄色。",
		"图表可视化效果中使用的橙色。",
		"图表可视化效果中使用的绿色。",
		"图表可视化效果中使用的紫色。",
	],
	"vs/platform/theme/common/iconRegistry": [
		"要使用的字体的 ID。如果未设置，则使用最先定义的字体。",
		"与图标定义关联的字体字符。",
		"小组件中“关闭”操作的图标。",
		"“转到上一个编辑器位置”图标。",
		"“转到下一个编辑器位置”图标。",
	],
	"vs/platform/undoRedo/common/undoRedoService": [
		"以下文件已关闭并且已在磁盘上修改: {0}。",
		"以下文件已以不兼容的方式修改: {0}。",
		"无法在所有文件中撤消“{0}”。{1}",
		"无法在所有文件中撤消“{0}”。{1}",
		"无法撤消所有文件的“{0}”，因为已更改 {1}",
		"无法跨所有文件撤销“{0}”，因为 {1} 上已有一项撤消或重做操作正在运行",
		"无法跨所有文件撤销“{0}”，因为同时发生了一项撤消或重做操作",
		"是否要在所有文件中撤消“{0}”?",
		"在 {0} 个文件中撤消",
		"撤消此文件",
		"取消",
		"无法撤销“{0}”，因为已有一项撤消或重做操作正在运行。",
		"是否要撤消“{0}”?",
		"是",
		"否",
		"无法在所有文件中重做“{0}”。{1}",
		"无法在所有文件中重做“{0}”。{1}",
		"无法对所有文件重做“{0}”，因为已更改 {1}",
		"无法跨所有文件重做“{0}”，因为 {1} 上已有一项撤消或重做操作正在运行",
		"无法跨所有文件重做“{0}”，因为同时发生了一项撤消或重做操作",
		"无法重做“{0}”，因为已有一项撤消或重做操作正在运行。",
	],
	"vs/platform/workspaces/common/workspaces": [
		"Code 工作区",
	]
});