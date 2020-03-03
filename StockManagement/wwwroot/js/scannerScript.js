window.JsFunctions = {
    scanner: function () {
        let selectedDeviceId;
        const codeReader = new ZXing.BrowserBarcodeReader()

        function resetVideo() {
            codeReader.decodeOnceFromVideoDevice(selectedDeviceId, 'video').then((result) => {
                console.log(result)
                document.getElementById('result').value = result.text
                document.getElementById('result').dispatchEvent(new Event("change"))
            }).catch((err) => {
                console.error(err)
            })
        }

        codeReader.getVideoInputDevices()
            .then((videoInputDevices) => {
                const sourceSelect = document.getElementById('sourceSelect')
                selectedDeviceId = videoInputDevices[1] ? videoInputDevices[1].deviceId : videoInputDevices[0].deviceId

                resetVideo()
                if (videoInputDevices.length > 1) {
                    videoInputDevices.forEach((element) => {
                        const sourceOption = document.createElement('option')
                        sourceOption.text = element.label
                        sourceOption.value = element.deviceId
                        sourceSelect.appendChild(sourceOption)
                        sourceSelect.value = selectedDeviceId
                    })

                    sourceSelect.onchange = () => {
                        selectedDeviceId = sourceSelect.value;
                        resetVideo()
                    }

                    const sourceSelectPanel = document.getElementById('sourceSelectPanel')
                    sourceSelectPanel.style.display = 'block'
                }

                document.getElementById('resetButton').addEventListener('click', () => {
                    document.getElementById('result').value = '';
                    codeReader.reset();
                    resetVideo()
                })

            })
            .catch((err) => {
                console.error(err)
            })
    },

    quagga: function () {
        Quagga.CameraAccess.enumerateVideoDevices()
            .then(function (devices) {
                console.log(devices)
            });

        var state = {
            inputStream: {
                type: "LiveStream",
                target: document.getElementById("interactive"),
                constraints: {
                    width: { min: 640 },
                    height: { min: 480 },
                    facingMode: "environment",
                    deviceId: "29ca6bf28b9d5612f77fbeab103b554488774ca78ede6ba389902b809850cf53",
                    aspectRatio: { min: 1, max: 2 },
                },
                
            }
        }

        Quagga.init(state, function (err) {
            if (err) {
                return self.handleError(err);
            }
            Quagga.start();
        });

        Quagga.onProcessed(function (result) {
            var drawingCtx = Quagga.canvas.ctx.overlay,
                drawingCanvas = Quagga.canvas.dom.overlay;

            if (result) {
                if (result.boxes) {
                    drawingCtx.clearRect(0, 0, parseInt(drawingCanvas.getAttribute("width")), parseInt(drawingCanvas.getAttribute("height")));
                    result.boxes.filter(function (box) {
                        return box !== result.box;
                    }).forEach(function (box) {
                        Quagga.ImageDebug.drawPath(box, { x: 0, y: 1 }, drawingCtx, { color: "green", lineWidth: 2 });
                    });
                }

                if (result.box) {
                    Quagga.ImageDebug.drawPath(result.box, { x: 0, y: 1 }, drawingCtx, { color: "#00F", lineWidth: 2 });
                }

                if (result.codeResult && result.codeResult.code) {
                    Quagga.ImageDebug.drawPath(result.line, { x: 'x', y: 'y' }, drawingCtx, { color: 'red', lineWidth: 3 });
                }
            }
        });

        Quagga.onDetected(function (result) {
            console.log(result)
        });  

    }
}