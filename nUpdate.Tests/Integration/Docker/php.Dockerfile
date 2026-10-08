FROM php:8.3-apache
RUN docker-php-ext-install mysqli && a2enmod headers
# Apache strips the Authorization header unless it is passed on explicitly.
RUN printf 'SetEnvIf Authorization "(.*)" HTTP_AUTHORIZATION=$1\n' > /etc/apache2/conf-enabled/authorization.conf
