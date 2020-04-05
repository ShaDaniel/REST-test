@all @delete_order
Feature: DeleteOrderPet

@020420200625 @DShapochkin
Scenario: Delete an order for a pet
	* create order for a pet and check in base
	* delete pet order
	* ensure code is 200
	* ensure that deletion order is ok
	* delete pet order
	* ensure code is 404
