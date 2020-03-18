window.JsFunctions = {
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
                    {
                        format: "ean_reader",
                        config: {
                            supplements: [
                                'ean_5_reader', 'ean_2_reader'
                            ]
                        }
                    },
                    //"code_93_reader",
                    //"ean_8_reader",
                    "upc_reader",
                    "upc_e_reader",
                    //"code_39_reader"
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

                $node = $('<li><div class="thumbnail"><div class="imgWrapper"><img /></div><div class="caption"><h5 class="code"></h5></div></div></li>');
                $node.find("img").attr("src", canvas.toDataURL());
                $node.find("h5.code").html(code);
                $("#result_strip ul.thumbnails").append($node);
                $node.on("click", function (e) {
                    document.getElementById("codeField").value = code;
                    document.getElementById('codeField').dispatchEvent(new Event("change"))
                })
            }
            document.getElementById("codeField").value = code;
            document.getElementById('codeField').dispatchEvent(new Event("change"))
        });

        Quagga.init(state, function (err) {
            if (err) {
                return console.log(err);
            }
            Quagga.start();
        });

    }
}