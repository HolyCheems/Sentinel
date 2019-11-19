Array.prototype.contains = function (elem) {
	for (let i = 0; i < this.length; i++) {
		if (this[i] === elem)
			return true;
	}
	return false;
};
Array.prototype.any = function (predicate) {
	for (let i = 0; i < this.length; i++) {
		if (predicate(this[i]))
			return true;
	}
	return false;
};
Array.prototype.min = function (predicate) {
	let min = { element: null, value: Infinity };
	for (let i = 0; i < this.length; i++) {
		let value = predicate(this[i]);
		if (value < min) {
			min.element = this[i];
			min.value = value;
		}
	}
	return min.element;
};
Array.prototype.minValue = function (predicate) {
	let min = Infinity;
	for (let i = 0; i < this.length; i++) {
		let value = predicate(this[i]);
		if (value < min) {
			min = value;
		}
	}
	return min;
};
Array.prototype.select = function (predicate) {
	let results = [];
	for (let i = 0; i < this.length; i++) {
		results.push(predicate(this[i]));
	}
	return results;
}
Array.prototype.where = function (predicate) {
	let results = [];
	for (let i = 0; i < this.length; i++) {
		if (predicate(this[i]))
			results.push(this[i]);
	}
	return results;
}
Array.prototype.remove = function (elem) {
	for (let i = 0; i < this.length; i++) {
		if (this[i] === elem)
			this.splice(i, 1);
	}
};
Array.prototype.find = function (predicate) {
	for (let i = 0; i < this.length; i++) {
		if (predicate(this[i]))
			return this[i];
	}
	return null;
};