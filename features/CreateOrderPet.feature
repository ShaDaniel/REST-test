@all @create_order
Feature: CreateOrderForPet
	(Отказались от негативных тестов, так как все равно вернет 200)

@020420200230 @pos @DShapochkin
Scenario: Positive test: create an order for a pet
		* create order for a pet and check in base
		* ensure code is 200
		* ensure order creation is ok
		* check if both messages match
