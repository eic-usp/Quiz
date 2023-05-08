# Quiz
Generic quiz asset for Unity.

The purpose of this asset is to provide a simple and flexible way to set up quiz mechanics. It can be used it as a minigame or as a main mechanic, just set it up and you are good to go!

## Installation

Download the latest release of the Unity asset package [here](https://github.com/eic-usp/Quiz/releases/latest) and import it into your project using the *Assets > Import Package* menu option. For more information on how to import asset packages, check [this page](https://docs.unity3d.com/Manual/AssetPackagesImport.html).

## Setting up question resources

To setup your questions database, create a JSON file inside a special [Resources](https://docs.unity3d.com/ScriptReference/Resources.Load.html) folder. Note that the questions inside a resource file will be picked randomly, however you can set the maximum number of questions and you can have multiple resource files. Having multiple resource files can be useful to separate the questions into different databases according to a theme or difficulty. 

```json
{
  "questions": [
    {
      "question": "(42 + 16) * 3 = ?", 
      "correctOption": 2, 
      "options": [
        "118", 
        "174", 
        "152", 
        "180"
      ]
    }, 
    {
      "question": "How many bones are there in the human body?", 
      "correctOption": 4, 
      "options": [
        "140", 
        "39", 
        "321", 
        "206"
      ]
    }
  ]
}
```

## Quiz Manager options

- `Right Answer Color`: the button highlight color on choosing the right option.
- `Wrong Answer Color`: same as above, but for the wrong options.
- `Questions File Path`: the resource file path, relative to the Resource folder, i.e. If you place your resource inside a folder named *Questions*, the path will be *Questions/file.json*.
- `Number Of Questions`: the number of questions to be used in the quiz. Assign 0 to use all the questions.
- `Multiple Attempts`: allow multiple failed attempts for each question. This is useful for didactic experiences.

## Getting started

First make a reference to your `QuizManager` object and assign it in the inspector, then set up subscription to the `QuizManager.OnChoose` event action.

```csharp
public class QuizDemoHandler : MonoBehaviour
{
    [SerializeField] private QuizManager quizManager;
    
    private void OnEnable()
    {
        quizManager.OnChoose += OnChoose;
    }

    private void OnDisable()
    {
        quizManager.OnChoose -= OnChoose;
    }
    ...
```

Then define your own `OnChoose` method as follows.

```csharp
    ...
    private void OnChoose(bool correct, QuizResult quizResult)
    {
        Debug.Log($"{(correct ? "Right" : "Wrong")} answer!");
        
        if (quizResult.complete)
        {
            Debug.Log("The end!");
            Debug.Log($"Right answers: {quizResult.rightAnswers} | Wrong answers: {quizResult.wrongAnswers}");
            return;
        }
        
        var qdi = quizManager.PopQuestion();
        Debug.Log($"Question: {qdi.question}");
    }
}
```

The transition will happen instantly, so you might want to set a delay coroutine in order to see the highlight colors.

```csharp
...

private IEnumerator NextQuestion(float seconds)
{
    yield return new WaitForSeconds(seconds);
    var qdi = quizManager.PopQuestion();
    Debug.Log($"Question: {qdi.question}");
}

...
```

And apply to your `OnChoose` method.

```csharp
    ...
    
    private void OnChoose(bool correct, QuizResult quizResult)
    {
        Debug.Log($"{(correct ? "Right" : "Wrong")} answer!");
        
        if (quizResult.complete)
        {
            Debug.Log("The end!");
            Debug.Log($"Right answers: {quizResult.rightAnswers} | Wrong answers: {quizResult.wrongAnswers}");
            return;
        }
        
        StartCoroutine(NextQuestion(1f));
    }
    
}
```

To use the refresh question method with an UI button, you will need to wrap it inside a void method. 

```csharp
...

public void RefreshQuestion()
{
    quizManager.RefreshQuestion();
}

...
```

The result:

## Checking the quiz result

The OnChoose event action signature is:

```csharp
public event System.Action<bool, QuizResult> OnChoose;
```

While the bool property indicates whether the chosen option is correct, the `QuizResult` struct includes counters for right and wrong answers and a bool value that indicates whether the result is partial or complete.

## Retrieving information about picked questions

You can retrieve info on each picked question using the `QuizManager.PopQuestion` method return value, as follows.

```csharp
var qdi = quizManager.PopQuestion();
Debug.Log($"Question: {qdi.question}");
```

The `QuizDataItem` object contains info about the question, including the available options and which one of them is correct.
This is useful for creating logs and summaries.