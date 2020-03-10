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
    var App = {
        init: function () {
            var self = this;

            Quagga.init(this.state, function (err) {
                if (err) {
                    return self.handleError(err);
                }
                //Quagga.registerResultCollector(resultCollector);

                Quagga.start();
            });
        },
        handleError: function (err) {
            console.log(err);
        },
        state: {
            inputStream: {
                type: "LiveStream",
                constraints: {
                    width: { min: 640 },
                    height: { min: 480 },
                    facingMode: "environment",
                    aspectRatio: { min: 1, max: 2 }
                }
            },
            locator: {
                patchSize: "medium",
                halfSample: true
            },
            numOfWorkers: 2,
            frequency: 2,
            decoder: {
                readers: [
                    "code_128_reader",
                    "ean_reader",
                    //"code_93_reader",
                    "ean_8_reader",
                    "upc_reader",
                    "upc_e_reader",
                    //"code_39_reader"
                ]
            },
            locate: true
        },
        found: []
    };

    App.init();

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
        var code = result.codeResult.code;
        
        if (!App.found.includes(code)) {
            App.found.push(code);
            var $node = null, canvas = Quagga.canvas.dom.image;

            $node = $('<li><div class="thumbnail"><div class="imgWrapper"><img /></div><div class="caption"><h4 class="code"></h4></div></div></li>');
            $node.find("img").attr("src", canvas.toDataURL());
            $node.find("h4.code").html(code);
            $("#result_strip ul.thumbnails").append($node);
            $node.on("click", function (e) {
                document.getElementById("codeField").value = code;
                document.getElementById('codeField').dispatchEvent(new Event("change"))
            })
        }
        document.getElementById("codeField").value = code;
        document.getElementById('codeField').dispatchEvent(new Event("change"))
    });

}
}