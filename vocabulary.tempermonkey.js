// ==UserScript==
// @name         Vocabulary Modifier and Uploader
// @namespace    http://tampermonkey.net/
// @version      1.0
// @description  Modify content on vocabulary.com and send data to your server.
// @author       You
// @match        https://www.vocabulary.com/dictionary/*
// @grant        none
// ==/UserScript==

(function () {
    'use strict';

    // Replace with your REST API and WebSocket server URLs
    const restApiUrl = 'https://your-server.com/api/endpoint';

    // Establish WebSocket connection
    const socket = new WebSocket('wss://localhost:7159/ws');

    socket.onopen = () => {
        console.log('WebSocket connected');
    };

    socket.onmessage = (event) => {
        const word = event.data;
        console.log('Received word from server:', word);
        window.location.replace("https://www.vocabulary.com/dictionary/"+word);
        
        // Use the word for searching on Vocabulary.com
        // searchAndReplaceWord(word, 'searched');
    };

    socket.onclose = () => {
        console.log('WebSocket disconnected');
    };

    $(document).ready(function () {
       const short = $(".word-area>.short").text();
       const long =$(".word-area>.long").text();
       console.log(short,long);
    });
})();
