.SHELL := /usr/bin/bash
.PHONY: init plan apply

BOLD=$(shell tput -T xterm bold)
MAGENTA=$(shell tput -T xterm setaf 5)
RESET=$(shell tput -T xterm sgr0)

init:
	@printf "\n${BOLD}${MAGENTA}***terraform init***${RESET}\n"
	@terraform init \
		-backend=true \
		-backend-config=remote.tfvars \
		-input=false

plan: init
	@printf "\n${BOLD}${MAGENTA}***terraform plan***${RESET}\n"
	@terraform plan \
		-input=false \
		-lock=true \
		-lock-timeout=60m \
		-refresh=true \
		-var-file global.tfvars

apply: init
	@printf "\n${BOLD}${MAGENTA}***terraform apply***${RESET}\n"
	@terraform apply \
		-input=false \
		-auto-approve=true \
		-lock=true \
		-lock-timeout=60m \
		-refresh=true \
		-var-file global.tfvars

destroy: init
	@printf "\n${BOLD}${MAGENTA}***terraform destroy***${RESET}\n"
	@terraform destroy \
		-input=false \
		-auto-approve=true \
		-lock=true \
		-lock-timeout=60m \
		-refresh=true \
		-var-file global.tfvars