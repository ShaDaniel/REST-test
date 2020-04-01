@all @pet_creation
Feature: Pet creation
Обязательные поля в модели: name, photoUrls

@010420202330 @pos
Scenario: Positive test: create pet
		* create pet with name "Cat" and photourls "test.com"
		* create pet with name "a" and photourls "."
@020420200018 @neg
Scenario: Negative test: create pet
		* create pet with name "" and photourls "test.com"
		* create pet with name "a" and photourls ""
		* send empty json body
