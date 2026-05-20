import { dotnet } from './_framework/dotnet.js'

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

// Override console.error to also store errors for debugging
if (typeof window !== 'undefined') {
    window.__avaloniaErrors = [];
    const origError = console.error;
    console.error = function() {
        window.__avaloniaErrors.push(Array.from(arguments).join(' '));
        origError.apply(console, arguments);
    };
    const origWarn = console.warn;
    console.warn = function() {
        window.__avaloniaErrors.push('[WARN] ' + Array.from(arguments).join(' '));
        origWarn.apply(console, arguments);
    };
    window.addEventListener('error', function(e) {
        window.__avaloniaErrors.push('[UNHANDLED] ' + e.message);
    });
    window.addEventListener('unhandledrejection', function(e) {
        window.__avaloniaErrors.push('[PROMISE] ' + e.reason);
    });
}

const dotnetRuntime = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

const config = dotnetRuntime.getConfig();

try {
    await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
} catch (err) {
    console.error('FATAL: Gallery.Browser runMain failed:', err);
    if (typeof window !== 'undefined') {
        window.__avaloniaErrors.push('[FATAL] ' + (err.message || err));
        // Also make errors visible on the page
        const errorDiv = document.createElement('div');
        errorDiv.style.cssText = 'position:fixed;top:0;left:0;right:0;bottom:0;background:white;color:red;padding:20px;z-index:9999;font-family:monospace;white-space:pre-wrap;overflow:auto';
        errorDiv.textContent = 'Gallery.Browser Error:\n\n' + window.__avaloniaErrors.join('\n');
        document.body.appendChild(errorDiv);
    }
}
