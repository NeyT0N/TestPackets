mergeInto(LibraryManager.library, {

    Check: function() {
        var result = window.navigator.onLine ? 'true' : 'false';

        console.log('###Checked internet connection. Result: network ' + (window.navigator.onLine ? 'connected' : 'not connected'))
        
        var bufferSize = lengthBytesUTF8(result) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(result, buffer, bufferSize);
        return buffer;
    }

});