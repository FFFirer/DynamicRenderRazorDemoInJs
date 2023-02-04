/**通知vscode已经加载页面完成 */
export function emitPageLoaded() {
    window.parent.postMessage(
        {
            "event": "loaded",
            "from": "server"
        },
        "*"
    );
}
