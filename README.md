# DarlMLRestExample
An example of using the free whitebox DARL Machine learning service.

This example is a simple console application that loads the data and specification for 3 different supervized learning examples and sends them off to the free DARL Machine learning service. The results of the learning process are emailed back to you. This includes a complete DARL ruleset that can be used to model the relationships learned from the data.

You will want to apply your own data, but the three examples demonstrate how to do this. Please note that the examples given are classification examples, but DARL can also predict numeric variables. 

The code is simple:
```C#
        static string destEmail = "support@darl.ai"; //put your email address here!
        static void Main(string[] args)
        {
            DarlML("yingyang").Wait();
            DarlML("iris").Wait();
            DarlML("cleveheart").Wait();
        }

        static async Task DarlML(string examplename)
        {
            var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream($"DarlMLRestExample.{examplename}.darl"));
            var source = reader.ReadToEnd();
            reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream($"DarlMLRestExample.{examplename}.xml"));
            var data = reader.ReadToEnd();
            var spec = new DarlMLData { code = source, data = data, email = destEmail, percentTrain = 100, sets = 5, jobName = examplename};//use your own choice of training percent (1-100) and sets, (3,5,7,9)
            var valueString = JsonConvert.SerializeObject(spec);
            var client = new HttpClient();
            var response = await client.PostAsync("https://darl.ai/api/Linter/DarlML", new StringContent(valueString, Encoding.UTF8, "application/json"));
            //check for errors here...
        }
```
For each example a data set and specification are read from the exe's embedded data and a _DarlMLData_ class is constructed containing the data and skeleton code along with a couple of simple parameters and the email to send the result to.

The machine learning process expands the input and output definitions with fuzzy sets and categories and creates rules that are inserted into the skeleton rule set.

The definition of _DarlMLData_ and information on the REST interface can be found at [darl.ai/swagger](http://www.darl.ai/swagger/).

# The examples

## Iris
This is the famous Fisher's Iris data containing 50 examples each of 3 cultivars of Iris. The data values are petal and sepal widths and lengths, the output is the cultivar. This is a surprisingly difficult data set, since the 3 types are not _linearly separable_ i.e. you could not efficiently separate them by placing planes in the 4 dimensional input space.

## YingYang
This is a two dimensional problem with two categories forming the shape of the Chinese Ying Yang symbol. Again they are not linearly separable. Try higher set values, like 7 & 9.

## Cleveheart
This is the cleveland heart data set containing data on pationts admitted following a heart attack. The predicted value is the outcome encoded as a set of integers. 


Machine Learning
===
You can machine learn DARL rulesets. Not only that, but the quality of machine learning is very high.
Furthermore, unlike many other machine learning algorithms, there's only one thing to adjust: the number of fuzzy sets to use to model numeric variables.

There is a massive potential ethical problem with Machine Learning, that is only beginning to be considered: what do you do when a misclassification or poor prediction damages somebody, by denying them a loan, or a job, or identifying them as a terrorist?
With DARL you machine learn fuzzy logic rules that you can understand and audit. With Deep Learning and other Neural Net techniques you create a "black box". The system has learned something, but you don't know what.
DARL is a __whitebox__ system in that the learned model is easily understood.

# Machine learning to a rule set
Configuration of machine learning is performed mostly within a rule set definition. You specify a ruleset, add the tag "supervised" to show that the system should modify the rule set, and specify the inputs and one output.
Our machine learning proceeds one output at a a time. If you want to train more you can just assemble the results of multiple training runs.
The inputs and output need to be decorated with a type. Only _Numeric_ and _Categorical_ are permitted for inputs and outputs in a machine learning run.

The machine learning system will examine the training data for categories and create fuzzy sets based on the density profile of numeric data.

A typical machine learning skeleton looks like this. 
```darl
ruleset iris supervised
{
	input numeric petal_length;
	input numeric sepal_length;
	input numeric petal_width;
	input numeric sepal_width;

	output categorical class;

}
``` 

The process of mining annotates the inputs and outputs with categories and sets, and then generates rules.
Comments are added to record the performance of the learning process.
```darl
ruleset iris supervised
{
    // Generated by DARL rule induction on  2/21/2018 8:59:23 PM.
    // Train correct:  92.76% on 152 patterns.
    // Percentage of unknown responses over all patterns: 0.00
    input numeric petal_length {{small, -∞,1,4.4},{medium, 1,4.4,6.9},{large, 4.4,6.9,∞}};
    input numeric petal_width {{small, -∞,0.1,1.3},{medium, 0.1,1.3,2.5},{large, 1.3,2.5,∞}};
    input numeric sepal_length {{small, -∞,4.3,5.8},{medium, 4.3,5.8,7.9},{large, 5.8,7.9,∞}};
    input numeric sepal_width {{small, -∞,2,3},{medium, 2,3,4.4},{large, 3,4.4,∞}};

    output categorical class {"Iris-setosa","Iris-versicolor","Iris-virginica"};

    if petal_width is small  then class will be "Iris-setosa" confidence 1; // examples: 50
    if petal_width is medium  and petal_length is medium  then class will be "Iris-versicolor" confidence 0.793650793650794; // examples: 63
    if petal_width is medium  and petal_length is large  then class will be "Iris-virginica" confidence 1; // examples: 4
    if petal_width is large  then class will be "Iris-virginica" confidence 1; // examples: 30
}
```
# Controls
There are two things you can control: 
## Fuzzy set count
Choices are 3,5,7and 9. The system will assign names to the sets that make reading easier. This determines the granularity of the machine learning for numeric inputs and outputs.

## Percent to train on
Where data is copious, it makes sense to have both a training and test set, since the performance of machine learning systems is typically worse on data that did not form part of the training set.
Permitted values are between 1 and 100% inclusive.
If you chose, say, 90 for this value, a randomly selected 90% is used for training, and the rest for testing.
The results for both sets are reported.

# Locating data
Data can be input in either XML or JSon.  In either case the data is tree shaped, and consists, generally of a sequence of similar or sub trees.
## Navigating data
For XML we use XPath to locate patterns and items, for Json, JSonPath.
## Locating patterns
Each training example is a pattern located by an expression. An example for XPath is: 

```darl
pattern "//Iris";
```
The pattern element is specified outside of the ruleset it relates to.

The patterns contain data items that must be associated with the inputs and output.

This is done using relative expressions from the pattern.

```xml
<?xml version = "1.0"?>
<irisdata>
	<Iris>
		<sepal_length>5.10</sepal_length>
		<sepal_width>3.50</sepal_width>
		<petal_length>1.40</petal_length>
		<petal_width>0.20</petal_width>
		<class>Iris-setosa</class>
	</Iris>
```

So given data looking like the above, where each Iris block corresponds to a pattern, we can access the individual data items by name.

## Missing data
Many machine learning algorithms demand _orthogonal_ data. That is data where every pattern has the same data values with none missing. 
Users of such algorithms are stuck with the problem of inserting dummy values where real values are not available.
Our machine learning algorithm can handle missing data without the need to create dummy values. Obviously the more data that is missing, the more the machine learning is likely to degrade, but a pattern with missing values can still be used for machine learning without any further treatment.


## Wiring
DARL follows a schematic paradigm, in that rulesets are treated like circuit elements that can be wired up.
You can read more [here](./darl).

_Mapinput_ and _mapoutput_are elements at the edge of the schematic sheet, and these wire up to the the data items.

The following code, again specified outside of any rule set, specifies edge inputs and outputs, their names and the respective relative XPath.
The Wire elements connect the edge elements to the rule set.
```darl
mapinput petal_length "petal_length";
mapinput petal_width "petal_width";	
mapinput sepal_length "sepal_length";
mapinput sepal_width "sepal_width";

mapoutput class "class";

wire petal_length iris.petal_length;
wire petal_width iris.petal_width;
wire sepal_length iris.sepal_length;
wire sepal_width iris.sepal_width;
wire iris.class class;
```

## Common mistakes

The principal mistake users make is to incorrectly specify the pattern path and the relative paths for the source data. If the machine learning system responds by telling you there is insufficient data, this is usually the reason.
Use an XML or Json editor to ensure that your paths are correct.

It is also entirely possible that there is nothing to be learned from a data set; i.e. that there is nothing in the data that can be used to predict the output. This will be evinced by poor results.

The choice of the number of fuzzy sets determines to a certain extent the complexity of the resulting model if numeric inputs or outputs are present.
A generalization of Occam's razor predicts that there is an optimum complexity for any model. This means that you may find that as you increase the number of fuzzy sets the performance on the training data may improve, but at some point the test data performance may start to get worse. You may therefore choose to experiment with the number of fuzzy sets.

Finally, Lotfi Zadeh, the now deceased inventor of Fuzzy Logic thought of it as _Granular_ computing. By this he meant that you could use relatively simple and few blocks to represent quite complex systems. This is one of the surprising things about fuzzy logic, that the models created are often much simpler and more robust than those created by purely boolean rule induction.
If you find yourself worrying too much about the exact definitions of fuzzy sets, you are probably applying machine learning to a system capable of analytic modeling. Machine learning is for noisy, ill defined or ephemeral relationships.
