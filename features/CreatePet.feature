@all @pet_creation
Feature: Pet creation
Обязательные поля в модели: name, photoUrls

@010420202330 @pos @DShapochkin
Scenario: Positive test: create pet
		* create pet with name "Cat" and photourls "test.com"
		* create pet with name "a" and photourls "."
@020420200018 @neg @DShapochkin
Scenario: Negative test: create pet
		* create pet with name "" and photourls "test.com"
		* create pet with name "a" and photourls ""
		* send empty json body
