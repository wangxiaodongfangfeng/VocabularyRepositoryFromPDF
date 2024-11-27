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
    // Establish WebSocket connection
    const socket = new WebSocket('wss://localhost:7159/ws');

    socket.onopen = () => {
        console.log('WebSocket connected');
    };

    socket.onmessage = (event) => {
        const word = event.data;
        console.log('Received word from server:', word);
        window.location.replace("https://www.vocabulary.com/dictionary/"+word);
    };

    socket.onclose = () => {
        console.log('WebSocket disconnected');
    };
    async function sendWordToServer(word,description) {
        try {
            const response = await fetch('https://localhost:7159/api/Words/Save', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ word,description,url:window.location.href }),
            });
            const result = await response.json();
            console.log('Word sent to server:', result);
        } catch (error) {
            console.error('Error sending word to server:', error);
        }
    }
    $(document).ready(async ()=> {
       const short = $(".word-area>.short").text();
       const long =$(".word-area>.long").text();
       const word = $("#hdr-word-area").text();
       
       if(word&&(short!=null||long!=null)) {
           await sendWordToServer(word.replaceAll("\t","").replaceAll("\n",""), short.replaceAll("\t","").replaceAll("\n",""));
       }
       console.log(short,long);
    });
})();
