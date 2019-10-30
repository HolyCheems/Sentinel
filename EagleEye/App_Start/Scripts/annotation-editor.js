class AnnotationEditor extends AnnotationViewer {
    constructor(lot, canvas) {
        super(lot, canvas, (self, ctx, annotation) => {
            ctx.strokeStyle = 'black';


            switch (annotation.Type) {
                case 'Parking': ctx.fillStyle = 'rgba(0,255,0,0.3)'; break;//red
                case 'Aisle': ctx.fillStyle = 'rgba(187,0,255,0.2)'; break;//dark blue
                case 'Select': ctx.fillStyle = 'rgba(0,0,192,0.25)'; break;//
                case 'Constant': ctx.fillStyle = 'rgba(255,255,255,0.2)'; break;//white
                case 'Handicap': ctx.fillStyle = 'rgba(0,0,255,0.3)'; break;//green 
                case 'Motorcycle': ctx.fillStyle = 'rgba(255,165,0,0.3)'; break;//Yellow

            }
            return true;
        }, function (self, ctx, annotation, point) {
            if (self.selected(point)) {
                ctx.fillStyle = 'blue';
                ctx.strokeStyle = 'blue';
            } else {
                ctx.fillStyle = 'black';
                ctx.strokeStyle = 'black';
            }
            return true;
        });
        // Selecting points
        this.shift = false;
        this.ctrl = false;
        this.selection = [];
        this.clipboard = [];
        // Drawing new annotations
        this.drawMode = 'Select';
        this.drawing = null; // the annotation being drawn
        // Setup event listeners
        this.dragging = null; // if the selection is being dragged
        this.hover = null; // the point being hovered over

        let select = (evt) => {
            if (this.drawMode === 'Select') {
                this.lot.Annotations.forEach(annotation => {
                    let selectedPoint = null;
                    annotation.Points.forEach(point => {
                        let screen = this.toScreen(point);
                        let x = screen.X - evt.offsetX;
                        let y = screen.Y - evt.offsetY;
                        if (x * x + y * y <= this.pointRadius * this.pointRadius) {
                            selectedPoint = point;
                        }
                    });
                    if (selectedPoint) {
                        if (!this.selected(selectedPoint)) {
                            if (this.shift) {
                                this.selection.push(selectedPoint);
                            } else {
                                this.selection = [selectedPoint];
                            }
                        }
                        this.dragging = selectedPoint;
                        return true;
                    }
                });
                if (this.dragging)
                    return;

            }
            this.drawing = new Annotation({ Type: this.drawMode });
            this.drawing.Points.push(new Vector2(evt.offsetX / this.canvas.width, evt.offsetY / this.canvas.height));
            this.drawing.Points.push(new Vector2(evt.offsetX / this.canvas.width, evt.offsetY / this.canvas.height));
            this.drawing.Points.push(new Vector2(evt.offsetX / this.canvas.width, evt.offsetY / this.canvas.height));
            this.drawing.Points.push(new Vector2(evt.offsetX / this.canvas.width, evt.offsetY / this.canvas.height));

            this.selection = [];
        };
        let stopSelect = () => {
            this.dragging = null;
            if (this.drawing !== null) {
                if (this.drawMode === 'Select') {
                    this.lot.Annotations.forEach(a => {
                        a.Points.forEach(p => {
                            if (this.drawing.contains(p)) {
                                this.selection.push(p);
                            }
                        });
                    });
                } else {
                    this.lot.Annotations.push(this.drawing);
                }
                this.drawing = null;
                this.drawMode = 'Select';
            }
        };
        let move = (evt) => {
            let x = evt.offsetX / this.canvas.width;
            let y = evt.offsetY / this.canvas.height;
            if (this.dragging && this.selection.length > 0) {
                let dx = x - this.dragging.X;
                let dy = y - this.dragging.Y;
                this.selection.forEach(p => {
                    p.X += dx;
                    p.Y += dy;
                });
            } else if (this.drawing) {
                this.drawing.Points[1].X = x;
                this.drawing.Points[3].Y = y;
                this.drawing.Points[2].X = x;
                this.drawing.Points[2].Y = y;
            }
            this.hover = null;
            this.canvas.style.cursor = 'default';
            this.lot.Annotations.forEach(annotation => {
                if (annotation.Points.forEach(point => {
                    let screen = this.toScreen(point);
                    let x = screen.X - evt.offsetX;
                    let y = screen.Y - evt.offsetY;
                    if (x * x + y * y <= this.pointRadius * this.pointRadius) {
                        this.canvas.style.cursor = 'pointer';
                        this.hover = point;
                        return true;
                    }
                }))
                    return true;
            });

        };
        function convertTouchEvent(evt) {
            let touch = evt.targetTouches[0];
            let canvasBounds = canvas.getBoundingClientRect();
            return {
                offsetX: touch.clientX - canvasBounds.left,
                offsetY: touch.clientY - canvasBounds.top
            };
        }
        this.canvas.addEventListener('touchstart', (evt) => {
            if (evt.targetTouches.length === 1) {
                evt.preventDefault();
                select(convertTouchEvent(evt));
            }
        });
        this.canvas.addEventListener('mousedown', (evt) => {
            if (evt.button === 0) {
                select(evt);
            }
        });
        window.addEventListener('touchend', (evt) => {
            stopSelect();
            this.hover = null;
        });
        window.addEventListener('mouseup', (evt) => {
            if (evt.button === 0) {
                stopSelect();
            }
        });
        this.canvas.addEventListener('touchmove', (evt) => {
            if (evt.targetTouches.length === 1) {
                evt.preventDefault();
                move(convertTouchEvent(evt));
            }
        });
        this.canvas.addEventListener('mousemove', (evt) => {
            move(evt);
        });
        window.addEventListener('keydown', (evt) => {
            // Shift
            if (evt.keyCode === 16) {
                this.shift = true;
            }
            // Delete
            else if (evt.keyCode === 46) {
                for (let i = 0; i < this.lot.Annotations.length; i++) {

                    let annotation = this.lot.Annotations[i];

                    if (this.selected(annotation)) {
                        this.lot.Annotations.splice(i, 1);
                        i--;
                    }
                }
            }
            // Ctrl
            else if (evt.keyCode === 17) {
                this.ctrl = true;
            }
            // C
            else if (evt.keyCode === 67) {
                if (this.ctrl) {
                    this.clipboard = [];
                    this.lot.Annotations.forEach(a => {
                        if (this.selected(a)) {
                            this.clipboard.push(a);
                        }
                    });
                }
            }
            // V
            else if (evt.keyCode === 86) {
                if (this.ctrl) {
                    this.selection = [];
                    this.clipboard.forEach(a => {
                        let annotation = {
                            ID: -1,
                            Points: [],
                            Type: a.Type
                        };
                        a.Points.forEach(p => {
                            let dx = 10 / this.canvas.width;
                            let dy = 10 / this.canvas.width;
                            let point = new Vector2(p.X + dx, p.Y + dy);
                            this.selection.push(point);
                            annotation.Points.push(point);
                        });
                        this.lot.Annotations.push(annotation);
                    });
                }
            }
        });
        window.addEventListener('keyup', (evt) => {
            if (evt.keyCode === 16) {
                this.shift = false;
            } else if (evt.keyCode === 17) {
                this.ctrl = false;
            }
        });

    }
    selected(obj) {
        if (obj instanceof Vector2) {
            for (let i = 0; i < this.selection.length; i++) {
                if (this.selection[i].X === obj.X && this.selection[i].Y === obj.Y) {
                    return true;
                }
            }
        } else {
            let allContained = true;
            for (let j = 0; j < obj.Points.length; j++) {
                if (!this.selected(obj.Points[j])) {
                    allContained = false;
                    break;
                }
            }
            return allContained;
        }
        return false;
    }

    save() {
        $.ajax({
            url: '/ParkingLot/Update',
            method: 'POST',
            data: this.lot
        }).done(data => {

        });
    }
    newSpace() {
        this.drawMode = 'Parking';
    }
	newMotorcycle() {
        this.drawMode = 'Motorcycle';
    }
    newAisle() {
        this.drawMode = 'Aisle';
    }
    newConstant() {
        this.drawMode = 'Constant';
    }
    newHandicap() {
        this.drawMode = 'Handicap';
    }



}