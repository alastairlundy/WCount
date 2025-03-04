console.log("JavaScript file loaded");

function setEditorFontSize(id, fontSize) {
	var element = document.getElementById(id);

	if (element) {
		document.getElementById(id).style.fontSize = fontSize + "pt";
	}
	else {
		printIdNotFoundWarn(id);
	}
}

function getEditorCaretPosition(id) {
	const element = document.getElementById(id);

	if (element) {
		return element.selectionEnd;
	}
	else {
		printIdNotFoundWarn(id);
	}
}

function printIdNotFoundWarn(id) {
	console.warn("Could not find element with id of: " + id);
}
function getClipboardText() {
	var text = navigator.clipboard.readText();

	if (text) {
		return text;
	}
	else {
		return "";
	}
}