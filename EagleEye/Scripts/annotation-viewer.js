class AnnotationViewer {
	constructor(lot, canvas, prepareAnnotationRender, preparePointRender) {
		this.lot = lot;
		this.canvas = canvas;
		this.prepareAnnotationRender = prepareAnnotationRender;
		this.preparePointRender = preparePointRender;
		// resize canvas to fit screen resolution
		this.canvas.width = this.canvas.clientWidth;
		this.canvas.height = this.canvas.clientHeight;
		// Display constants 
		this.lineWidth = 1;
		this.pointRadius = 8;
		this.pointDisplayRadius = 2;
		let mat = new Matrix([
			[7, 2, 1],
			[0, 3, -1],
			[-3,4,-2]
		]);
		console.log(mat.inverse);
	}
	get start() {
		return this.m_start;
	}
	set start(value) {
		this.m_start = value;
		if (this.goal)
			this.setPath(this.aStar(this.start, this.goal));
	}
	get goal() {
		return this.m_goal;
	}
	set goal(value) {
		this.m_goal = value;
		if (this.start)
			this.setPath(this.aStar(this.start, this.goal));
	}
	setPath(path) {
		// Cubic spline
		//let Dy = null;
		//let Dx = null;
		//let cfs = new Matrix(path.length - 1, path.length - 1);
		//for (let i = 0; i < cfs.rowCount; i++) {
		//	for (let j = 0; j < cfs.columnCount; j++) {
		//		if (i === 0 && j === 0 || i === cfs.rowCount - 1 && j === cfs.columnCount - 1) {
		//			cfs.set(i, j, 2);
		//		} else if (i === j) {
		//			cfs.set(i, j, 4);
		//		} else if (j - 1 === i || i - 1 === j) {
		//			cfs.set(i, j, 1);
		//		}
		//	}
		//}
		//console.log(cfs);
		//{
			
		//	let vector = [];
		//	for (let i = 0; i < path.length - 1; i++) {
		//		vector.push(3 * (path[i + 1].X - path[i].X));
		//	}
		//	Dx = cfs.inverse.multiplyVector(vector);
		//}
		//{
		//	let vector = [];
		//	for (let i = 0; i < path.length - 1; i++) {
		//		vector.push(3 * (path[i + 1].Y - path[i].Y));
		//	}
		//	Dy = cfs.inverse.multiplyVector(vector);
		//}
		//let spline = [];
		//function interpolate(points,d,t,i) {
		//	return points[i] + d[i] * t + (3 * (points[i + 1] - points[i]) - 2 * d[i] - d[i + 1]) * t * t + (2 * (points[i] - points[i + 1]) + d[i] + d[i + 1]) * t * t * t;
		//}
		//for (let i = 0; i < path.length - 1; i++) {
		//	for (let t = 0; t < 1; t += 0.1) {
		//		spline.push(new Vector2(
		//			interpolate(path.select(p => p.X), Dx, t,i),
		//			interpolate(path.select(p => p.Y), Dy, t,i)
		//		));
		//	}
		//}
		this.path = path;
	}
	toScreen(point) {
		return {
			X: this.canvas.width * point.X, Y: this.canvas.height * point.Y
		};
	}
	tick() {
		this.render();
		window.requestAnimationFrame(() => {
			this.tick();
		});
	}
	render() {
		let self = this;
		let ctx = this.canvas.getContext('2d');
		ctx.strokeStyle = 'black';
		ctx.setLineDash([]);
		ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
		function renderAnnotation(ctx, annotation,fill,stroke,points) {
			
			
			ctx.beginPath();
			annotation.Points.forEach((point, i) => {
				let screen = self.toScreen(point);
				if (i === 0) {
					ctx.moveTo(screen.X, screen.Y);
				} else {
					ctx.lineTo(screen.X, screen.Y);
				}
			});
			ctx.closePath();
			if (fill)
				ctx.fill();
			if (stroke)
				ctx.stroke();
			
			if (points) {
				annotation.Points.forEach((point, i) => {
					if (self.preparePointRender(self, ctx, annotation, point)) {
						let screen = self.toScreen(point);
						ctx.beginPath();
						ctx.arc(screen.X, screen.Y, point === self.hover ? self.pointRadius : self.pointDisplayRadius, 0, Math.PI * 2);

						ctx.fill();
						ctx.stroke();
					}
				});
			}
		}
		this.lot.Annotations.forEach(annotation => {
			if (self.prepareAnnotationRender(self, ctx, annotation)) {
				renderAnnotation(ctx, annotation, true, false,true);
			}
		});
		this.lot.Annotations.forEach(annotation => {
			if (self.prepareAnnotationRender(self, ctx, annotation)) {
				renderAnnotation(ctx, annotation, false, true, true);
			}
		});
		if (this.drawMode !== undefined && this.drawMode !== 'Select') {
			ctx.fillStyle = 'rgba(255,255,255,0.5)';
			ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
			if (this.drawing) {
				if (self.prepareAnnotationRender(self, ctx, this.drawing)) {
					renderAnnotation(ctx, this.drawing, true, true, true);
				}
			}
		} else if (this.drawing) {
			ctx.setLineDash([5, 5]);
			ctx.strokeStyle = 'blue';
			ctx.fillStyle = 'rgba(0,0,255,0.25)';
			renderAnnotation(ctx, this.drawing,true,true,false);
		}

		if (this.path) {
			ctx.beginPath();
			ctx.strokeStyle = 'blue';
			ctx.lineWidth = '8';
			ctx.lineCap = 'round';
			ctx.lineJoin = 'round';
			this.path.forEach((point, i) => {
				let screen = self.toScreen(point);
				if (i === 0) {
					ctx.moveTo(screen.X, screen.Y);
				} else {
					ctx.lineTo(screen.X, screen.Y);
				}
			});
			ctx.stroke();
		}
		// Render the start point
		if (this.start) {
			if (self.preparePointRender(self, ctx, null, this.start)) {
				let screen = self.toScreen(this.start);
				ctx.beginPath();
				ctx.arc(screen.X, screen.Y, this.start === self.hover ? self.pointRadius : self.pointDisplayRadius, 0, Math.PI * 2);

				ctx.fill();
				ctx.stroke();
			}
		}

	}
	generateNodes() {
		let nodes = [];
		this.lot.Annotations.where(an => an.Type === "Aisle").forEach(an => {
			let midpoints = an.midpoints;
			for (let i = 0; i < 2; i++) {
				let start = midpoints[i];
				let end = midpoints[i + 2];
				let segment = end.subtract(start);
				let step = segment.length / Math.ceil(segment.length / AnnotationViewer.nodeDistance);
				for (let t = 0; t <= segment.length; t += step) {
					nodes.push(start.add(segment.normalized().scale(t)));
				}
			}
			//let xAxes = [
			//	an.Points[1].subtract(an.Points[0]).normalized(),
			//	an.Points[2].subtract(an.Points[3]).normalized()
			//];
			//let yAxes = [
			//	an.Points[3].subtract(an.Points[0]).normalized(),
			//	an.Points[2].subtract(an.Points[1]).normalized()
			//];
			//for (let xt = 0; xt <= 1; xt += 1 / 8) {
			//	for (let yt = 0; yt <= 1; yt += 1 / 8) {
			//		let yStart = xAxes[0].scale(xt).add(an.Points[0]);
			//		let yEnd = xAxes[1].scale(xt).add(an.Points[3]);
					
						
			//		let xStart = yAxes[0].scale(yt).add(an.Points[0]);
			//		let xEnd = yAxes[1].scale(yt).add(an.Points[1]);
			//		if (yStart.dot(xStart) < 0) {
			//			let temp = yStart;
			//			yStart = yEnd;
			//			yEnd = temp;
			//		}
			//		let t = xStart.subtract(yStart).cross(yEnd.scale(1 / xEnd.cross(yEnd)));
			//		nodes.push(xEnd.subtract(xStart).normalized().scale(t).add(xStart));
			//	}
			//}
			
		});
		return nodes;
	}
	
	aStarReconstructPath(cameFrom, current) {
		let totalPath = [current];
		while (cameFrom.get(current)) {
			current = cameFrom.get(current);
			totalPath.push(current);
		}
		return totalPath;
	}
	aStarHeuristicCostEstimate(a, b) {
		if (a === b) {
			return 0;
		} else {
			return a.subtract(b).length;
		}
	}
	aStarNeighbors(nodes, a, goalAnnotation) {
		return nodes.where(n => {
            return n !== a && !this.lot.Annotations.any(an => an.ID !== goalAnnotation.ID && an.Type !== 'Aisle' && an.intersects(a, n));
		});
	}
	static get nodeDistance() {
		return 0.025;
	}

	aStar(start, goalAnnotation) {
		let goal = goalAnnotation.midpoints;
		let nodes = this.generateNodes();
		nodes.push(start);
		goal.forEach(n => nodes.push(n));
		// The set of nodes already evaluated
		let closedSet = [];

		// The set of currently discovered nodes that are not evaluated yet.
		// Initially, only the start node is known.
		let openSet = [start];

		// For each node, which node it can most efficiently be reached from.
		// If a node can be reached from many nodes, cameFrom will eventually contain the
		// most efficient previous step.
		let cameFrom = new HashMap();

		// For each node, the cost of getting from the start node to that node.
		let gScore = new HashMap();
		nodes.forEach(n => {
			gScore.set(n, Infinity);
		});
		
		// The cost of going from start to start is zero.
		gScore.set(start, 0);

		// For each node, the total cost of getting from the start node to the goal
		// by passing by that node. That value is partly known, partly heuristic.
		let fScore = new HashMap();
		nodes.forEach(n => {
			fScore.set(n, Infinity);
		});
		

		// For the first node, that value is completely heuristic.
		fScore.set(start,this.aStarHeuristicCostEstimate(start, goal[0]));

		let current = start;
		while (openSet.length > 0) {
			let min = { node: null, fScore: Infinity };
			for (let i = 0; i < openSet.length; i++) {
				let value = fScore.get(openSet[i]);
				if (value <= min.fScore) {
					min = { node: openSet[i], fScore: value };
				}
			}
			current = min.node;

			if (goal.contains(current))
				return this.aStarReconstructPath(cameFrom, current);

			openSet.remove(current);

			closedSet.push(current);

			this.aStarNeighbors(nodes, current, goalAnnotation).forEach(neighbor => {

				if (closedSet.contains(neighbor))
					return; // Ignore the neighbor which is already evaluated.

				// The distance from start to a neighbor
				let tentativeGScore = gScore.get(current) + current.subtract(neighbor).length;


				if (!openSet.contains(neighbor)) {  // Discover a new node
					openSet.push(neighbor);
				}
				else if (tentativeGScore >= gScore.get(neighbor)) {
					return;
				}

				// This path is the best until now. Record it!
				cameFrom.set(neighbor, current);

				gScore.set(neighbor,tentativeGScore);

				fScore.set(neighbor, tentativeGScore + goal.minValue(g => this.aStarHeuristicCostEstimate(neighbor, g)));
			});
			
		}
		// A valid path was not found, return the best found path
		return this.aStarReconstructPath(cameFrom, current);
		
	}
}
class HashMap {
	constructor() {
		this.map = {};
	}
	set(key, value) {
		this.map[JSON.stringify(key)] = value;
	}
	get(key) {
		return this.map[JSON.stringify(key)];
	}
	remove(key) {
		delete this.map[JSON.stringify(key)];
	}
}
class Vector2 {
	constructor(x, y) {
		this.X = x;
		this.Y = y;
	}
	add(b) {
		return new Vector2(this.X + b.X, this.Y + b.Y);
	}
	scale(scalar) {
		return new Vector2(this.X * scalar, this.Y * scalar);
	}
	subtract(b) {
		return new Vector2(this.X - b.X, this.Y - b.Y);
	}
	dot(b) {
		return b.X * this.X + b.Y * this.Y;
	}
	cross(b) {
		return this.X * b.Y - this.Y * b.X; 
	}
	get normal() {
		return new Vector2(-this.Y, this.X);
	}
	normalized() {
		let length = this.length;
		return new Vector2(this.X / length, this.Y / length);
	}
	negate() {
		return new Vector2(-this.X, -this.Y);
	}
	crossMag(b) {
		return this.X * b.Y - this.Y * b.X;
	}
	tripleProduct(a, b) {
		return this.cross(a.crossMag(b));
	}
	get length() {
		return Math.sqrt(this.X * this.X + this.Y * this.Y);
	}
	static get unitX() {
		return new Vector2(1, 0);
	}
	static get unitY() {
		return new Vector2(0, 1);
	}
}
class Matrix {
	constructor(rowCount, columnCount) {
		if (typeof rowCount === 'number' && typeof columnCount === 'number') {
			this.rowCount = rowCount;
			this.columnCount = columnCount;
			this.data = [];
			for (let rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				let row = [];
				for (let columnIndex = 0; columnIndex < columnCount; columnIndex++) {
					row.push(0);
				}
				this.data.push(row);
			}
		} else {
			this.data = rowCount;
			this.rowCount = this.data.length;
			try {
				this.columnCount = this.data[0] ? this.data[0].length : 0;
			} catch (ex) {
				console.log(ex);
			}
		}
	}
	get(row,column) {
		return this.data[row][column];
	}
	set(row, column, value) {
		this.data[row][column] = value;
	}
	get determinant() {
		let det = 0;
		let submatrix = new Matrix(this.rowCount - 1, this.columnCount - 1);
		if (this.rowCount === 0) {
			return 0;
		}
		else if (this.rowCount === 1) {
			return this.get(0, 0);
		}
		else if (this.rowCount === 2) {
			return (this.get(0,0) * this.get(1,1)) - (this.get(1,0) * this.get(0,1));
		} else {
			for (let x = 0; x < this.rowCount; x++) {
				let subi = 0;
				for (let i = 1; i < this.rowCount; i++) {
					let subj = 0;
					for (let j = 0; j < this.rowCount; j++) {
						if (j === x)
							continue;
						submatrix.set(subi, subj, this.get(i, j));
						subj++;
					}
					subi++;
				}
				det = det + (Math.pow(-1, x) * this.get(0, x) * submatrix.determinant);
			}
		}
		return det;
	}
	get transpose() {
		let transpose = new Matrix(this.columnCount,this.rowCount);
		for (let i = 0; i < this.rowCount; i++)
		{
			for (let j = 0; j < this.columnCount; j++)
			{
				transpose.set(j, i, this.get(i, j));
			}
		}
		return transpose;
	}
	get minors() {
		let cofactor = new Matrix(this.rowCount,this.columnCount);
		for (let i = 0; i < this.rowCount; i++)
		{
			for (let j = 0; j < this.columnCount; j++)
			{
				let subMatrix = [];
				for (let iSub = 0; iSub < this.rowCount; iSub++) {
					if (iSub !== i) {
						let row = [];
						for (let jSub = 0; jSub < this.columnCount; jSub++) {
							if (jSub !== j) {
								row.push(this.get(iSub, jSub));
							}
						}
						subMatrix.push(row);
					}
				}
				subMatrix = new Matrix(subMatrix);
				cofactor.set(i, j,subMatrix.determinant);
			}
		}
		return cofactor;
	}
	get adjugate() {
		let adjugate = new Matrix(this.rowCount,this.columnCount);
		let index = 0;
		if (this.rowCount === 0) {

		} else if (this.rowCount === 1) {
			adjugate.set(0, 0, this.get(0, 0));
		}
		if (this.rowCount === 2) {
			adjugate.set(0, 0, this.get(0, 0));
			adjugate.set(0, 1, - this.get(0,1));
			adjugate.set(1, 0, - this.get(1, 0));
			adjugate.set(1, 1, this.get(1, 1));
		} else {
			for (let i = 0; i < this.rowCount; i++) {
				for (let j = 0; j < this.columnCount; j++) {
					if (index % 2 === 1) {
						adjugate.set(i, j, -this.get(i, j));
					} else {
						adjugate.set(i, j, this.get(i, j));
					}
					index++;
				}
			}
		}
		return adjugate;
	}
	multiplyVector(vector) {
		let result = [];
		for (let row = 0; row < this.rowCount; row++) {
			result.push(this.dot(this.data[row], vector));
		}
		return result;
	}
	scale(scalar) {
		let result = new Matrix(this.rowCount, this.columnCount);
		for (let i = 0; i < this.rowCount; i++) {
			for (let j = 0; j < this.columnCount; j++) {
				result.set(i, j, this.get(i, j) * scalar);
			}
		}
		return result;
	}
	dot(A,B) {
		let dot = 0;
		A.forEach((a, i) => dot += a * B[i]);
		return dot;
	}
	get inverse() {
		let det = this.determinant;
		if (det !== 0) {
			return this.transpose.minors.adjugate.scale(1 / det);
		}
		// Inverse does not exist
		return null;
	}
	static createIdentity(size) {
		let identity = new Matrix(rowCount, columnCount);
		for (let i = 0; i < size; i++) {
			identity.set(i, i, 1);
		}
		return identity;
	}
}
class Annotation {
	constructor(props) {
		this.ID = -1;
		this.Points = [];
		for (let prop in props) {
			this[prop] = props[prop];
		}
	}
	get area() {
		return this.triangleArea(this.Points[0], this.Points[1], this.Points[2])
			+ this.triangleArea(this.Points[0], this.Points[2], this.Points[3]);
	}
	triangleArea(a, b, c) {
		let ab = b.subtract(a);
		return ab.length * 0.5 * Math.abs(ab.normal.normalized().dot(c.subtract(a)));
	}
	contains(point) {
		return SAT(this.Points, [point]);
	}
	intersects(a, b) {
		let annotationPolygon = new Polygon(this.Points[3],this.Points[2],this.Points[1],this.Points[0]);
		return GJK(annotationPolygon, new Swept(new Polygon(a, b), new Circle(new Vector2(0, 0), .01)));
		//return SAT(this.Points, [a, b]);
	}
	get midpoints() {
		let midpoints = [];
		for (let i = 0; i < 4; i++) {
			let a = this.Points[i];
			let b = this.Points[(i + 1) % 4];
			midpoints.push(a.add(b).scale(0.5));
		}
		return midpoints;
	}
}
function GJK(a, b) {
	let s = new Simplex();
	s.add(new MinkowskiDifference(a, b, Vector2.unitY));
	let dir = s.a.negate().normalized();
	let it = 0;
	do {
		let support = new MinkowskiDifference(a, b, dir);
		if (support.dot(dir) < 0)
			return false;
		s.add(support);
		it++;
	} while (it < 100 && !(() => {
		switch (s.size) {
			case 1: {
				dir = s.a.negate().normalized();
				return false;
			}
			case 2: {
				let ab = s.b.subtract(s.a);

				if (ab.dot(s.b) >= 0) {
					let ao = s.a.negate();
					dir = ab.normal.negate().normalized();
					if (dir.dot(ao) < 0) {
						dir = dir.negate();
						// flip the winding to be ccw
						let temp = s.a;
						s.vertices[0] = s.vertices[1];
						s.vertices[1] = s.vertices[0];
					}
				}
				return false;
			}
			case 3: {
				let ab = s.b.subtract(s.a);
				let ca = s.a.subtract(s.c);
				let bc = s.c.subtract(s.b);
				let ao = s.a.negate();
				dir = ca.normal.normalized();
				if (dir.dot(ao) > 0) {
					s.vertices.splice(1, 1);
				} else {
					dir = bc.normal.normalized();
					if (dir.dot(s.c.negate()) > 0) {
						s.vertices.splice(0, 1);
						// flip the winding to be ccw
						let temp = s.a;
						s.vertices[0] = s.vertices[1];
						s.vertices[1] = s.vertices[0];
					} else {
						return true;
					}
				}
				return false;
			}
		}
		return true;
	})());
	return s;
}
class Convex {
	constructor() {
	}
	support(dir) {
	}
	translate(vector) {
	}
	rotate(pivot, radians) {
	}
	get area() { return 0; }
	get centroid() { }
}
class MinkowskiDifference extends Vector2 {
	constructor(a, b, dir) {
		super();
		try {
			this.a = a.support(dir);
			this.b = b.support(dir.negate());
			this.X = this.a.X - this.b.X;
			this.Y = this.a.Y - this.b.Y;
		} catch (ex) {
			console.log(ex);
		}
	}
}
class Simplex {
	constructor() {
		this.vertices = [];
	}
	add(vertex) {
		this.vertices.push(vertex);
	}
	get size() {
		return this.vertices.length;
	}
	get a() {
		return this.vertices[0];
	}
	get b() {
		return this.vertices[1];
	}
	get c() {
		return this.vertices[2];
	}
	render(ctx) {

		ctx.beginPath();
		let screen = game.worldToScreen(this.vertices[0]);
		ctx.moveTo(screen.x, screen.y);
		for (let i = 1; i <= this.vertices.length; i++) {
			screen = game.worldToScreen(this.vertices[i % this.vertices.length]);
			ctx.lineTo(screen.x, screen.y);
		}
		ctx.fill();
		ctx.stroke();
	}
}
class Polygon extends Convex {
	constructor(...vertices) {
		super();
		this.vertices = vertices;
	}
	add(vertex) {
		this.vertices.push(vertex);
	}
	support(dir) {
		let maxDot = -Infinity;
		let maxSupport = null;
		this.vertices.forEach(p => {
			let dot = p.dot(dir);
			if (dot > maxDot) {
				maxDot = dot;
				maxSupport = p;
			}
		});
		return maxSupport;
	}
	translate(vector) {
		this.vertices.forEach((p, i) => {
			this.vertices[i] = p.add(vector);
		});
	}
	rotate(pivot, radians) {
		this.translate(pivot.negate());
		this.vertices.forEach((p, i) => {
			this.vertices[i] = Matrix.rotation(radians).transformVector(p);
		});
		this.translate(pivot);
	}
	get centroid() {
		let centroid = new Vector2();
		this.vertices.forEach(p => centroid = centroid.add(p));
		return centroid.scale(1 / this.vertices.length);
	}
	get area() {
		let area = 0;
		for (let i = 1; i < this.vertices.length - 1; i++) {
			let base = this.vertices[i].subtract(this.vertices[0]);
			let height = Math.abs(base.normal.normalized().dot(this.vertices[i + 1].subtract(this.vertices[0])));
			area += 0.5 * base.length * height;
		}
		return area;
	}
	get moment() {
		let moment = 0;
		let centroid = this.centroid;
		this.translate(centroid.negate());
		let bottom = 0;
		this.vertices.forEach((v, i) => {
			bottom += 6 * this.vertices[(i + 1) % this.vertices.length].crossMag(v);
		});
		this.vertices.forEach((v, i) => {
			let u = this.vertices[(i + 1) % this.vertices.length];
			moment += u.crossMag(v) * (v.dot(v) + v.dot(u) + u.dot(u));
		});

		this.translate(centroid);
		return moment / bottom;
	}
	render(ctx) {

		ctx.beginPath();
		let screen = game.worldToScreen(this.vertices[0]);
		ctx.moveTo(screen.x, screen.y);
		for (let i = 1; i <= this.vertices.length; i++) {
			screen = game.worldToScreen(this.vertices[i % this.vertices.length]);
			ctx.lineTo(screen.x, screen.y);
		}
		ctx.fill();
		ctx.stroke();

	}
}
class Swept extends Convex {
	constructor(a, b) {
		super();
		this.A = a;
		this.B = b;
	}
	support(dir) {
		return this.A.support(dir).add(this.B.support(dir));
	}
	translate(vector) {
		this.A.translate(vector);
		this.B.translate(vector);
	}
	rotate(pivot, radians) {
		this.A.rotate(pivot, radians);
		this.B.rotate(pivot, radians);
	}
	get area() { return 0; }
	get centroid() { }
}
class Circle extends Convex {
	constructor(center, radius) {
		super();
		this.center = center || new Vector2();
		this.radius = radius || 1;
	}
	support(dir) {
		return this.center.add(dir.scale(this.radius));
	}
	translate(vector) {
		this.center = this.center.add(vector);
	}
	rotate(pivot, radians) {
		this.translate(pivot.negate());
		this.center = Matrix.rotation(radians).transformVector(this.center);
		this.translate(pivot);
	}
	get centroid() {
		return this.center;
	}
	get area() {
		return this.radius * this.radius * Math.PI;
	}
	get moment() {
		return this.radius * this.radius * 0.5;
	}
	render(ctx) {
		ctx.beginPath();
		let screen = game.worldToScreen(this.center);
		let radius = this.radius * game.scale;
		ctx.arc(screen.x, screen.y, radius, 0, Math.PI * 2, true);
		ctx.fill();
		ctx.stroke();
	}
}
function SAT(A, B) {
	let axes = [];
	if (A.length > 1)
		A.forEach((p, i) => {
			axes.push(p.subtract(A[(i + 1) % A.length]).normal.normalized());
		});
	if (B.length > 1)
		B.forEach((p, i) => {
			try {
				axes.push(p.subtract(B[(i + 1) % B.length]).normal.normalized());
			} catch (ex) {
				console.log(ex);
			}
		});
	let intersection = true;
	axes.forEach(axis => {
		let projA = { min: Infinity, max: -Infinity };
		let projB = { min: Infinity, max: -Infinity };
		A.forEach(p => {
			let proj = p.dot(axis);
			if (proj < projA.min)
				projA.min = proj;
			if (proj > projA.max)
				projA.max = proj;
		});
		B.forEach(p => {
			let proj = p.dot(axis);
			if (proj < projB.min)
				projB.min = proj;
			if (proj > projB.max)
				projB.max = proj;
		});
		if (!(projA.min < projB.max && projB.min < projA.max)) {
			intersection = false;
			return true;
		}
	});
	return intersection;
}