@all @pet_creation
Feature: Pet creation
Обязательные поля в модели: name, photoUrls

@010420202330 @pos @DShapochkin
Scenario: Positive test: create pet
		* create pet with name "<name>" and photourls "<photourls>"
		Examples: 
		| name | photourls |
		| Cat  | test.com  |
		| a    | .         |

@020420200018 @neg @DShapochkin
Scenario: Negative test: create pet (1)
		* create pet with name "" and photourls "test.com"
		Examples: 
		| name | photourls |
		|      | test.com  |
		| a    |           |

@020420200020 @neg @DShapochkin
Scenario: Negative test: create pet (2)
		* send empty json body