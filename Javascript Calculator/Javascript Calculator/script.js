/*
	The Remove Function
*/

var remove = function(me) {
	me.remove();
}

/*
	The Calculate Funtion. The function checks the values that the user put in and then prints out the answer.
	If the user did not put in an operand it will throw an error message	
*/

var calculate = function() {
	var timeStamp = new Date().toLocaleDateString("en-US") + " " + new Date().toLocaleTimeString("en-US")
	var first = document.getElementById('numberOne').value;
	var second = document.getElementById('numberTwo').value;
	var temp = document.getElementById('operatorSelector');
	var operation = temp.options[temp.selectedIndex].text;


	if(first && second ){
	first = parseFloat(first);
	second = parseFloat(second);
	var equation = first + operation + second;
	
	var color = document.getElementById('color').value;
	var answerDiv = document.createElement('div');
	answerDiv.setAttribute('style', `background: ${color}; border-color: ${color}`);
	answerDiv.setAttribute('class', 'stuff-box');
	var answerPara = document.createElement('p');
	answerPara.textContent = timeStamp + `: ${first} ${operation} ${second} = ` + eval(equation);
	answerDiv.appendChild(answerPara);
	document.body.insertBefore(answerDiv, document.body.firstElementChild.nextElementSibling.nextElementSibling);
	answerDiv.setAttribute('onclick', "remove(this)");

} else {
	var answerDiv = document.createElement('div');
	answerDiv.setAttribute('class', 'red stuff-box');
	var answerPara = document.createElement('p');
	answerPara.textContent = timeStamp + " ERROR! Missing One Or More Operands!";
	answerDiv.appendChild(answerPara);
	document.body.insertBefore(answerDiv, document.body.firstElementChild.nextElementSibling.nextElementSibling);
	answerDiv.setAttribute('onclick', "remove(this)");
}
}


document.title = "JavaScript Calculator";

document.body.setAttribute('style', 'background: rgb(32,29,29)')

var div = document.createElement('div');
div.setAttribute('class', "black shadowed stuff-box ") ;
document.body.appendChild(div);

var h1 = document.createElement('h1');
h1.textContent = "JavaScript Calculator";
div.appendChild(h1);

var p = document.createElement('p');
div.appendChild(p);
p.textContent = "Create An Expression";
div.appendChild(p);


/*
	The First Number the user will put in	
*/
var numberOne = document.createElement('input')
numberOne.setAttribute('id', 'numberOne')
numberOne.setAttribute('type', 'number')
numberOne.setAttribute('placeholder', '3')
div.appendChild(numberOne)



/*
Creates the Operator Selector for the calculator	
*/

var operatorSelector = document.createElement('select');
operatorSelector.setAttribute('id', 'operatorSelector');

var operations = ["+","-","*","**","/","%"]

var fillSelector = function() {
	for(var i of operations) {
		var operation = document.createElement('option');
		operation.textContent = i;
		operatorSelector.options.add(operation);
	}
}

fillSelector();
div.appendChild(operatorSelector);


/*
	The Second Number the user will put in
*/

var numberTwo = document.createElement('input')
numberTwo.setAttribute('id', 'numberTwo')
numberTwo.setAttribute('type', 'number')
numberTwo.setAttribute('placeholder', '11')
div.appendChild(numberTwo)


/*
	Sets up the Compute Button	
*/
var calculateButton = document.createElement('button');
calculateButton.textContent = "Compute";
calculateButton.setAttribute('onclick', "calculate()")
div.appendChild(calculateButton);


/*
	Sets up the color Selector
*/

var color = document.createElement('input');
color.setAttribute('type', 'color');
color.setAttribute('id', 'color');

var colorText = document.createElement('p');
colorText.textContent = "Color of the New Div Box ";
colorText.appendChild(color);
div.appendChild(colorText);




