window.JsFunctions = {
    goBack: function () {
        window.history.go(-1);
    },
    toggleTooltips: function () {
        $('[data-toggle="tooltip"]').tooltip()
    },
    quagga: function () {
        Quagga.offProcessed();
        Quagga.offDetected();
        var state = {
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
            numOfWorkers: 4,
                frequency: 2,
                    decoder: {
                readers: [
                    "code_128_reader",
                    "ean_reader",
                    "ean_8_reader",
                    "upc_reader",
                    "upc_e_reader"
                ]
            },
            locate: true
        }

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

        var found = []
        Quagga.onDetected(function (result) {
            var code = result.codeResult.code;

            if (!found.includes(code)) {
                found.push(code);
                var $node = null, canvas = Quagga.canvas.dom.image;

                $node = $('<li><div class="thumbnail"><div class="imgWrapper"><img /></div><div class="caption"><p class="code"></p></div></div></li>');
                $node.find("img").attr("src", canvas.toDataURL());
                $node.find("p.code").html(code);
                $("#result_strip ul.thumbnails").append($node);
                $node.on("click", function (e) {
                    document.getElementById("codeField").value = code;
                    document.getElementById('codeField').dispatchEvent(new Event("change"))
                    found.pop(found.indexOf(code))
                    $node.remove()
                })

            }
            document.getElementById("codeField").value = code;
            document.getElementById('codeField').dispatchEvent(new Event("change"))
        });

        let _zoom = 1;
        let stream = document.getElementById("interactive");
        stream.onclick = function () {
            let track = Quagga.CameraAccess.getActiveTrack();
            if (track && typeof track.getCapabilities === 'function') {
                var capabilities = track.getCapabilities();
                if (capabilities.zoom) {
                    track.applyConstraints({ advanced: [{ zoom: _zoom % 3 }] })
                    zoom++;
                }
                
            }
        } 

        Quagga.init(state, function (err) {
            if (err) {
                return console.log(err);
            }
            Quagga.start();
        });
    }
}