
// JavaScript source code
function begin() {

    const vid = document.querySelector('video');
    navigator.mediaDevices.getUserMedia({ video: true }) // request cam
        .then(stream => {
            vid.srcObject = stream; // don't use createObjectURL(MediaStream)
            return vid.play(); // returns a Promise
        })
        .then(() => {
            repeatingFunct();
        })
        .catch(e => console.log('Something went wrong when starting the camera.'));
}

function takeASnap() {
	const vid = document.querySelector('video');
    const canvas = document.createElement('canvas'); // create a canvas
    const ctx = canvas.getContext('2d'); // get its context
	canvas.width = 200; // set its size to the one of the video
    canvas.height = (canvas.width / vid.videoWidth) * vid.videoHeight;
	ctx.drawImage(vid, 0, 0, vid.videoWidth, vid.videoHeight, 0, 0, canvas.width, canvas.height); // the video
    return new Promise((res, rej) => {
        canvas.toBlob(res, 'image/jpeg'); // request a Blob from the canvas
    });
}

function send(blob) {
	if (blob instanceof Blob) {
		var reader = new FileReader();
		reader.onloadend = () => {
			var text = btoa(String.fromCharCode.apply(null, new Uint8Array(reader.result)));
			$.ajax({
				url: "/Camera/Update",
				data: {
					ID: -1,
					Name: document.getElementById('cameraName').value,
					CurrentImage: text
				},
				method: "POST"
			}).done(() => {
				window.setTimeout(() => {
					repeatingFunct();
				}, 1000);

			});
		};
		reader.readAsArrayBuffer(blob);
	} else {
		window.setTimeout(() => {
			repeatingFunct();
		}, 1000);
	}
}

function repeatingFunct() {
    takeASnap().then(send);
}
