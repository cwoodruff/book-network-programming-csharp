// wwwroot/webrtc.js

let localConnection = null;
let remoteConnection = null;

window.webRTCInterop = {
    initializeConnection: function () {
        localConnection = new RTCPeerConnection();
        remoteConnection = new RTCPeerConnection();

        // Handle ICE candidates
        localConnection.onicecandidate = (event) => {
            if (event.candidate) {
                remoteConnection.addIceCandidate(event.candidate);
            }
        };

        remoteConnection.onicecandidate = (event) => {
            if (event.candidate) {
                localConnection.addIceCandidate(event.candidate);
            }
        };

        // Handle remote stream
        remoteConnection.ontrack = (event) => {
            const remoteVideo = document.getElementById("remoteVideo");
            remoteVideo.srcObject = event.streams[0];
        };
    },

    startCall: async function () {
        const stream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
        const localVideo = document.getElementById("localVideo");
        localVideo.srcObject = stream;

        stream.getTracks().forEach(track => localConnection.addTrack(track, stream));

        const offer = await localConnection.createOffer();
        await localConnection.setLocalDescription(offer);
        await remoteConnection.setRemoteDescription(offer);

        const answer = await remoteConnection.createAnswer();
        await remoteConnection.setLocalDescription(answer);
        await localConnection.setRemoteDescription(answer);
    },

    addTrack: async function() {
        // Assuming you already have an RTCPeerConnection object
        let peerConnection = new RTCPeerConnection({
            iceServers: [{ urls: 'stun:stun.l.google.com:19302' }]
        });

        // Obtain the media stream from user media (audio/video)
        navigator.mediaDevices.getUserMedia({ video: true, audio: true })
            .then(stream => {
                // Add each track to the peer connection
                stream.getTracks().forEach(track => {
                    peerConnection.addTrack(track, stream);
                });
                console.log('Tracks added to the peer connection.');
            })
            .catch(error => {
                console.error('Error accessing media devices.', error);
            });
    },
    
    removeTrack: async function(trackId) {
        // Assuming you already have an RTCPeerConnection object
        let peerConnection = new RTCPeerConnection({
            iceServers: [{ urls: 'stun:stun.l.google.com:19302' }]
        });
        
        // Assumes you have a global RTCPeerConnection instance named 'peer'
        const senders = peerConnection.getSenders();
        const sender = senders.find(s => s.track && s.track.id === trackId);
        if (sender) {
            peer.removeTrack(sender);
            console.log(`Track with id ${trackId} removed`);
        } else {
            console.log(`Track with id ${trackId} not found`);
        }
    }
};