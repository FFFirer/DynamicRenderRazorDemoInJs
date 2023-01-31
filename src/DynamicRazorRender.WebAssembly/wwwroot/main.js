const assemblyName = "DynamicRazorRender.Shared"
async function Run() {
    const code = document.querySelector("#code-area").value;
    
    // console.log(code);

    await DotNet.invokeMethodAsync(assemblyName, "CompileRazor", code);
}